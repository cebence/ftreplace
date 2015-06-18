rem OK: Replace text in a single file.

@rem Create a test file with some content.
@echo Just a few KEY notes.>test-2-result.txt

..\build\bin\debug\ftreplace -o test-2-result.txt -f KEY -r LOCK --debug

@pause
