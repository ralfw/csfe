package main

import (
	"reflect"
	"testing"
)

func TestNegate(t *testing.T) {
	// arrange
	testcases := []struct {
		in  []string
		out []string
	}{
		{
			in:  []string{"10", "1", "5", "7"},
			out: []string{"10", "-1", "-5", "7"},
		},
		{
			in:  []string{},
			out: []string{},
		},
		{
			in:  []string{"1", "2"},
			out: []string{"-1", "2"},
		},
		{
			in:  []string{"2", "1"},
			out: []string{"2", "1"},
		},
	}

	for i, tt := range testcases {
		// act
		actual := Negate(tt.in)
		expected := tt.out

		// assert
		if !reflect.DeepEqual(expected, actual) {
			t.Errorf("testcase:%d\nNegate() failed!\nexpected:%v\n  actual:%v\n", i, expected, actual)
		}
	}

}
