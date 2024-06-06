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

    #region Jarzabek

    public const string ZadanieDomowe1_1 = """
                                           procedure Circle {
                                              t = 1;
                                              a = t + 10;
                                              d = t * a + 2;
                                              call Triangle;
                                              b = t + a;
                                              call Hexagon;
                                              b = t + a;
                                              if t then {
                                                  k = a - d;
                                                  while c {
                                                      d = d + t;
                                                      c = d + 1; 
                                                  }
                                                  a = d + t; 
                                              }
                                              else {
                                                  a = d + t;
                                                  call Hexagon;
                                                  c = c - 1;
                                              }
                                              call Rectangle; 
                                           }
                                           procedure Rectangle {
                                              while c {
                                                  t = d + 3 * a + c;
                                                  call Triangle;
                                                  c = c + 20; 
                                              }
                                              d = t; 
                                           }
                                           procedure Triangle {
                                              while d {
                                              if t then {
                                                  d = t + 2; 
                                              }
                                              else {
                                                  a = t * a + d + k * b; }}
                                                  c = t + k + d;
                                              }
                                           procedure Hexagon {
                                              t = a + t; 
                                              hexagon = 1;
                                              Triangle = hexagon;
                                           }
                                           """;

    public const string ZadanieDomowe1 = """ 
                                         procedure Circle {
                                            t = 1;
                                            a = t + 10;
                                            d = t * a + 2;
                                            call Triangle;
                                            b = t + a;
                                            call Hexagon;
                                            b = t + a;
                                            if t then {
                                                k = a - d;
                                                while c {
                                                    d = d + t;
                                                    c = d + 1; 
                                                }
                                                a = d + t; 
                                            }
                                            else {
                                                a = d + t;
                                                call Hexagon;
                                                c = c - 1;
                                            }
                                            call Rectangle; 
                                         }
                                         procedure Rectangle {
                                            while c {
                                                t = d + 3 * a + c;
                                                call Triangle;
                                                c = c + 20; 
                                            }
                                            d = t; 
                                         }
                                         procedure Triangle {
                                            while d {
                                            if t then {
                                                d = t + 2; 
                                            }
                                            else {
                                                a = t * a + d + k * b; }}
                                                c = t + k + d;
                                            }
                                         procedure Hexagon {
                                            t = a + t; 
                                         }
                                         """;

    #endregion

    #region CallCode

    public const string CallCode = """
                                   procedure Circle {
                                       t = 1;
                                       call Triangle;
                                       b = t + a;
                                       call Rectangle;
                                       if t then {
                                           while c {
                                               if t then {
                                                   call Triangle;
                                               } 
                                               else {
                                                   call Rectangle;
                                               }
                                           }
                                           a = d + t; 
                                       }
                                       else {
                                           a = d + t;
                                       }
                                   }

                                   procedure Rectangle {
                                       while c {
                                           call Triangle;
                                           c = c + 20; 
                                       }
                                   }

                                   procedure Triangle {
                                       while d {
                                           if t then {
                                               d = t + 2; 
                                           }
                                           else {
                                               a = t * a + d + k * b; 
                                           }
                                       }
                                       c = t + k + d;
                                   }
                                   """;

    #endregion
}