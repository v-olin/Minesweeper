using System;
using System.Collections.Generic;
using System.Linq;

List<int> iss = Enumerable.Range(0,2).ToList();
List<int> iss2 = Enumerable.Range(-1,3).ToList();

foreach (var i in iss)
    System.Console.WriteLine(i + " ");

foreach (var i in iss2)
    System.Console.WriteLine(i + " ");