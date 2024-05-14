namespace Parsobober.Tests;

/// <summary>
/// Simple code snippets for testing purposes.
/// </summary>
public static class Code
{
    #region Short

    public const string ShortStatementsOnly = """
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

    #endregion

    #region Long

    public const string LongStatementsOnly = """
                               procedure First {
                                   d = z;
                                   while y{
                                       x = a +c+5;
                                       z = b+b+g;
                                       while a{
                                           t = p;
                                           while cos{
                                               essa = teressa;
                                               costam = git;
                                           }
                                           while cos2 {
                                               costam = eesa;
                                           }
                                           x = d;
                                       }
                                       r = b+m+w;
                                   }
                                   d = z;
                                   while z{
                                       a = z;
                                       while x{
                                           a = b;
                                           b = a;
                                       }
                                       while k{
                                           k = z;
                                           z = a;
                                       }
                                       a = b;
                                   }
                                   c = d;
                               }
                               """;

    #endregion
}