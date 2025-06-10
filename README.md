# Illumina Software Engineer Coding Assignment

Coding Assignment App is a console application that accepts a file name and prints out the key-value data in the file. The application currently supports the `.csv` file extension.

### Purpose
The purpose of this coding assignment is to assess your ability to:

- Understand the requirements, constraints and objectives of the given problem.
- Apply OOP principles such as encapsulation, inheritance or polymorphism when applicable.
- Identify and apply appropriate design patterns such as singleton, factory, strategy, etc to address the problem.
- Write testable codes with techniques such as dependency injection and separate of concerns.

**Note**: During the interview, the candidate is expected to provide a code walkthrough on their preferred IDE and share their screen for this purpose.

### Getting Started
Open `src/CodingAssignment.sln` with your preferred IDE and implement the following tasks:

### Task 1: Print data of various file extensions

1. Add support to print out key-value data of the `data.json` and `data.xml` files in the following format.
```
Data:
Key:aaaaa Value:bbbbb
Key:ccccc Value:ddddd
Key:eeeee Value:fffff
```
2. Create unit tests for any classes added for this feature.

### Task 2: Search and print data 

1. Prompt the user to input the search term for the key value.
2. The search term must match the beginning of the key value, but the search should be case-insensitive.
3. Search the data files in the `/data` directory of this repository.
4. Print the key-value data along with the file path(s) of the data file(s) that contains the key value.
5. E.g. Given the search term to search the key value is "aAa", the expected result is 
```
Key:aaaaa Value:bbbbb FileName:data\data.csv 
Key:aaaaa Value:bbbbb FileName:data\data2\data2.csv
Key:AAAYLQtn7V Value:CTDhy3riwv FileName:data\data2\data2.xml
```
6. Create unit tests to test the search feature.

Good luck!
