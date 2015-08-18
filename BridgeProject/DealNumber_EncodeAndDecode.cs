using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace BridgeProject
{
    class DealNumber
    {
        // ------------------------------------- http://bridge.thomasoandrews.com/impossible/algorithm.html ------------------------------------------------
        //
        // ... todo ... 
        //
        //


        // ----------------------------------------------------- http://www.rpbridge.net/7z68.htm ---------------------------------------------

        /*public String Deal_Decrypt(String strId)
        {
            // 1. N=E=S=W=13;  C=52;  K=D
            // 2. X=K*N/C;  If I < X then N=N-1, go to 6
            // 3. I=I-X;  X=K*E/C;  If I < X then E=E-1, go to 6
            // 4. I=I-X;  X=K*S/C;  If I < X then S=S-1, go to 6
            // 5. I=I-X;  X=K*W/C;  W=W-1
            // 6. K=X;  C=C-1, loop if not zero to 2

            int N, E, S, W, C;
            N = E = S = W = 13;
            C = 52;
            Decimal K, X, I, D;
            D = Decimal.Parse("53644737765488792839237440000");
            I = Decimal.Parse(strId);
            String res = "";

            int i = 0;
            K = D;
        start:
            X = K * ((Decimal)N / C);
            if (I < X)
            {
                i++;
                res += "N";
                //if (i % 4 == 0) res += " ";
                N = N - 1;
                goto fuck;
            }
            I = I - X;
            X = K * ((Decimal)E / C);
            if (I < X)
            {
                i++;
                res += "E";
                //if (i % 4 == 0) res += " ";
                E = E - 1;
                goto fuck;
            }
            I = I - X;
            X = K * ((Decimal)S / C);
            if (I < X)
            {
                i++;
                res += "S";
                //if (i % 4 == 0) res += " ";
                S = S - 1;
                goto fuck;
            }
            I = I - X;
            X = K * ((Decimal)W / C);
            i++;
            res += "W";
            //if (i % 4 == 0) res += " ";
            W = W - 1;

        fuck:
            K = X;
            C = C - 1;
            if (C != 0)
                goto start;
            else
                return res;
        }



        public Decimal Deal_Encrypt(String str)
        {
            int N, E, S, W, C;
            N = E = S = W = 13;
            C = 52;
            Decimal K, X, I, D;
            D = Decimal.Parse("53644737765488792839237440000");
            I = 0;

            int i = 0;
            K = D;
        start:
            X = K * ((Decimal)N / C);
            if (str[i] == 'N')
            {
                N = N - 1;
                goto fuck;
            }
            I = I + X;
            X = K * ((Decimal)E / C);
            if (str[i] == 'E')
            {
                E = E - 1;
                goto fuck;
            }
            I = I + X;
            X = K * ((Decimal)S / C);
            if (str[i] == 'S')
            {
                S = S - 1;
                goto fuck;
            }
            I = I + X;
            X = K * ((Decimal)W / C);
            W = W - 1;

        fuck:
            i++;
            K = X;
            C = C - 1;
            if (C != 0)
                goto start;
            else
                return I;
        }*/
    }
}
