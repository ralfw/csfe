def temp_name(file_name='', ext='', dir=nil)
    id   = Time.new.usec
    name = "%s%d.%s" % [file_name, id, ext]
    dir ? File.join(dir, name) : name
end


filenames = Dir["input/*"]
filenames.each{|filename| 
	sum = 0
	File.open(filename, "r").each_line do |valueLine|
		value = Integer(valueLine)
		sum += value
	end

	outputFilename = temp_name("", "txt", "output")
	File.open(outputFilename, 'w') { |file| file.write(sum) }
}