rem FAIL: Destination file is read-only.

@rem Create a read-only file.
@echo Can't touch this.>read-only.txt
@attrib +r read-only.txt

..\build\bin\debug\ftreplace -o read-only.txt -f KEY -r LOCK --debug

@pause
