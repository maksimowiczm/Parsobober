namespace Parsobober.Tests.Sprint1;

public abstract class Sprint1BaseTestClass(string code) : BaseTestClass(code)
{
    protected const string Code1 = """
                                procedure First {
                                  while y {
                                      x = a +c+5;
                                      z = b+b+g;
                                      while a {
                                        t = p;
                                      }
                                      r = b+m+w;
                                   }
                                }
                                """;
    //    procedure First { 
    // 1.    while y {
    // 2.      x = a +c+5;
    // 3.      z = b+b+g;
    // 4.      while a {
    // 5.        t = p;
    //         }
    // 6.      r = b+m+w;
    //       }
    //    }
}