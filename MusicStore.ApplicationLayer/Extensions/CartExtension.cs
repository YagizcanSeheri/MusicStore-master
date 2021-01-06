using System;
using System.Collections.Generic;
using System.Text;

namespace MusicStore.ApplicationLayer.Extensions
{
    public static class CartExtension
    {
        public static double GetPriceBaseOnQuantity(int quantity, double price, double price50, double price100)
        {
            if (quantity<50)
            {
                return price;
            }
            else
            {
                if (quantity<100)
                {
                    return price50;
                }
                else
                {
                    return price100;
                }
            }
        }

        public static string ConvertToRawHtml(string description)
        {
            char[] array = new char[description.Length];
            int arrayIndex = 0;
            bool inside = false;
            for (int i = 0; i < description.Length; i++)
            {
                char let = description[i];
                if (let =='<')
                {
                    inside = true;
                    continue;

                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array,0,arrayIndex);
        }

    }
}
