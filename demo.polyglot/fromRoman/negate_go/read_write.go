package main

import (
	"bufio"
	"fmt"
	"io/ioutil"
	"log"
	"os"
)

func read() []Job {
	jobs := []Job{}
	filenames := readFilenames()
	for _, filename := range filenames {
		lines := readFile(filename)
		jobs = append(jobs, Job{Filename: filename, Lines: lines})
	}
	return jobs
}

func readFilenames() []string {
	filenames := []string{}

	files, err := ioutil.ReadDir("input")
	if err != nil {
		log.Fatal(err)
	}

	for _, file := range files {
		filenames = append(filenames, file.Name())
	}

	return filenames
}

func readFile(filename string) []string {
	path := fmt.Sprintf("input/%s", filename)
	file, err := os.Open(path)
	defer file.Close()
	if err != nil {
		panic(err)
	}

	lines := []string{}
	scanner := bufio.NewScanner(file)
	scanner.Split(bufio.ScanLines)
	for scanner.Scan() {
		lines = append(lines, scanner.Text())
	}

	return lines
}

func write(jobs []Job, deleteInputAfterWrite bool) {
	for _, job := range jobs {
		outputFilename := fmt.Sprintf("output/%s", job.Filename)
		err := ioutil.WriteFile(outputFilename, job.LinesToByte(), 0644)
		if err != nil {
			panic(err)
		}

		if (deleteInputAfterWrite) {
			inputFilename := fmt.Sprintf("input/%s", job.Filename)
			os.Remove(inputFilename)
		}
	}
}
