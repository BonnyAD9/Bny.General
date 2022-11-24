using Bny.General;

Ptr<int> arr = new int[] { 0, 1, 2, 3, 4, 5, 6 };

foreach (ref var i in arr)
    i *= 2;

foreach (var i in arr)
    Console.WriteLine(i);