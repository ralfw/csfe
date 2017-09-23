package main

import (
	"strconv"
)

func main() {
	jobs := read()
	for i, _ := range jobs {
		jobs[i].Lines = Negate(jobs[i].Lines)
	}
	write(jobs)
}

func Negate(in []string) []string {
	if len(in) == 0 {
		return []string{}
	}
	out := make([]string, len(in))
	for i := 0; i < len(in); i++ {
		if i == 0 {
			continue
		}
		a, err := strconv.ParseInt(in[i-1], 10, 64)
		if err != nil {
			panic(err)
		}
		b, err := strconv.ParseInt(in[i], 10, 64)
		if err != nil {
			panic(err)
		}
		if a < b {
			out[i-1] = strconv.FormatInt(-1*a, 10)
		} else {
			out[i-1] = in[i-1]
		}
	}
	out[len(in)-1] = in[len(in)-1]
	return out
}
