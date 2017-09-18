using System.Diagnostics;
using System.Linq;
using csfe.compilation;



using System;

namespace csfe.compilation.parsing {



internal class Parser {
	public const int _EOF = 0;
	public const int _identifier = 1;
	public const int _tag = 2;
	public const int _stringliteral = 3;
	public const int maxT = 9;

	const bool _T = true;
	const bool _x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

public string[] RegisteredServicenames;
public ServiceFlowLangNode ASTroot;




	public Parser(Scanner scanner) {
		this.scanner = scanner;
		errors = new Errors();
	}

	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}

	
	void ServiceFlowLang() {
		this.ASTroot = new ServiceFlowLangNode();
		
		OperationList(this.ASTroot.Sequence);
	}

	void OperationList(OperationListNode opNodeList) {
		Operation(opNodeList);
		while (la.kind == 4) {
			Get();
			Operation(opNodeList);
		}
	}

	void Operation(OperationListNode opNodeList) {
		if (la.kind == 1) {
			Service(opNodeList);
		} else if (la.kind == 5) {
			Switch(opNodeList);
		} else SynErr(10);
	}

	void Service(OperationListNode opNodeList) {
		var op = new ServiceNode();
		
		servicename(op);
		if (la.kind == 5) {
			Get();
			Expect(3);
			op.Argument = t.val.Substring(0,t.val.Length-1).Substring(1); 
			
			Expect(6);
		}
		opNodeList.Operations.Add(op); 
	}

	void Switch(OperationListNode opNodeList) {
		var swtch = new SwitchNode(); 
		Expect(5);
		Option(swtch);
		while (la.kind == 7) {
			Get();
			Option(swtch);
		}
		Expect(6);
		opNodeList.Operations.Add(swtch); 
	}

	void servicename(ServiceNode op) {
		Expect(1);
		if (this.RegisteredServicenames != null
		   && !this.RegisteredServicenames.Contains(t.val))
		   this.errors.SemErr(t.line, t.col, $"Unregistered service found: '{t.val}'");
		op.Name = t.val; 
		
		if (la.kind == 8) {
			Get();
			Expect(1);
			op.Instance = t.val; 
		}
	}

	void Option(SwitchNode swtch) {
		Expect(2);
		var opt = new SwitchOptionNode();
		// strip trailing ':'
		opt.Tag = t.val.Substring(0,t.val.Length-1);
		
		OperationList(opt.Sequence);
		swtch.Options.Add(opt); 
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		ServiceFlowLang();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x}

	};
} // end Parser


public class Errors {
	public int count = 0;                                    // number of errors detected
	public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
	public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

	public virtual void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "identifier expected"; break;
			case 2: s = "tag expected"; break;
			case 3: s = "stringliteral expected"; break;
			case 4: s = "\">\" expected"; break;
			case 5: s = "\"(\" expected"; break;
			case 6: s = "\")\" expected"; break;
			case 7: s = "\",\" expected"; break;
			case 8: s = "\".\" expected"; break;
			case 9: s = "??? expected"; break;
			case 10: s = "invalid Operation"; break;

			default: s = "error " + n; break;
		}
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public virtual void SemErr (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}
	
	public virtual void SemErr (string s) {
		errorStream.WriteLine(s);
		count++;
	}
	
	public virtual void Warning (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
	}
	
	public virtual void Warning(string s) {
		errorStream.WriteLine(s);
	}
} // Errors


internal class FatalError: Exception {
	public FatalError(string m): base(m) {}
}
}