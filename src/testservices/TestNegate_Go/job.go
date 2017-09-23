package main

import "fmt"

type Job struct {
	Filename string
	Lines    []string
}

func (j *Job) LinesToByte() []byte {
	out := []byte{}
	for _, line := range j.Lines {
		lineWithNewline := fmt.Sprintf("%s\n", line)
		out = append(out, []byte(lineWithNewline)...)
	}
	return out
}
