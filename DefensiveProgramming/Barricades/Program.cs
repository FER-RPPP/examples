using Barricades;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

Course c = new Course("RPPP");
c["Pero"] = 2;
c["Ana"] = 5;
c["Ivan"] = 5;
c["Marko"] = 1;
c["Luka"] = 3;
//c["Marija"] = -7; //should throw an exception
Console.WriteLine("Average grade: " + c.AverageGrade());
Console.WriteLine("Šime has grade: " + c["Šime"]); //prints -1