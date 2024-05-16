# BitstampOrderBook

BitstampOrderBook is a service that provides the ability to save and retrieve the order book of the Bitstamp cryptocurrency exchange accounting, as well as calculate the value of cryptocurrency according to a given amount.

![зображення](https://github.com/Demihov/BitstampOrderBook/assets/32543463/932c68da-15b8-43fb-8c37-0d5c110ebb8d)

## Contents

1. [Installation](#installation)
2. [Tests](#tests)
3. [Assignment](#assignment)

## installation

1. Clone the repository:

```bash
git clone https://github.com/Demihov/BitstampOrderBook.git
```

2. Go to the project directory:

```bash
cd BitstampOrderBook
```
   3. Install the necessary packages using NuGet:

```bash
dotnet restore
```
4. Run the program:

```bash
dotnet run
```
## Tests

Run the tests to verify that the program is working correctly:

```bash
dotnet test
```

## Assignment
BitstampOrderBook was created to simplify the process of obtaining and analyzing the order book on the Bitstamp platform. Key features include:

- Saving the order book at a certain time
- Getting the order book by timestamp
- Calculation of the value of cryptocurrency according to the given amount

> [!IMPORTANT]
> All data is updated in real time
