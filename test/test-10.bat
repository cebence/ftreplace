rem OK: Replace 'user', 'USER', 'UsEr' etc. into 'User'.

@rem Create a test file with some content.
@echo user1, USER2, uSeR3, USEr4...>test-10-result.txt

..\build\bin\debug\ftreplace -o test-10-result.txt -f user -r User --ignore-case --debug

@pause
