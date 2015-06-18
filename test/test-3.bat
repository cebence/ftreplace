rem OK: Simple file copy.

@rem Create a test file with some content.
@echo Just a few KEY notes.>test-3-input.txt

..\build\bin\debug\ftreplace -i test-3-input.txt -o test-3-result.txt --debug

@pause
