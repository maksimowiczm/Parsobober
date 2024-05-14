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
    //    procedure First {
    // 1.     d = z;
    // 2.     while y{
    // 3.         x = a +c+5;
    // 4.         z = b+b+g;
    // 5.         while a{
    // 6.             t = p;
    // 7.             while cos{
    // 8.                 essa = teressa;
    // 9.                 costam = git;
    //                }
    // 10.            while cos2 {
    // 11.                costam = eesa;
    //                }
    // 12.            x = d;
    //            }
    // 13.        r = b+m+w;
    //        }
    // 14.    d = z;
    // 15.    while z{
    // 16.        a = z;
    // 17.        while x{
    // 18.            a = b;
    // 19.            b = a;
    //            }
    // 20.        while k{
    // 21.            k = z;
    // 22.            z = a;
    //            }
    // 23.        a = b;
    //        }
    // 24.    c = d;
    //    }

    #endregion
}