rem OK: Replace text from one to another file.

@rem Create a test file with some content.
@echo Just a few KEY notes.>test-1-input.txt

..\build\bin\debug\ftreplace -i test-1-input.txt -o test-1-result.txt -f KEY -r LOCK --debug

@pause
