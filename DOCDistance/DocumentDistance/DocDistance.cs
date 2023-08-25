using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Threading;

namespace DocumentDistance
{
    class DocDistance
    {
        // *****************************************
        // DON'T CHANGE CLASS OR FUNCTION NAME
        // YOU CAN ADD FUNCTIONS IF YOU NEED TO
        // *****************************************
        /// <summary>
        /// Write an efficient algorithm to calculate the distance between two documents
        /// </summary>
        /// <param name="doc1FilePath">File path of 1st document</param>
        /// <param name="doc2FilePath">File path of 2nd document</param>
        /// <returns>The angle (in degree) between the 2 documents</returns>
        public static double CalculateDistance(string doc1FilePath, string doc2FilePath)
        {
            // TODO comment the following line THEN fill your code here
            //throw new NotImplementedException();


            // read doc. 1
            Task<Dictionary<string, long>> wordsOfDoc1 = new Task<Dictionary<string, long>>(() => ReadDocuments(doc1FilePath));

            // read doc. 2
            Task<Dictionary<string, long>> wordsOfDoc2 = new Task<Dictionary<string, long>>(() => ReadDocuments(doc2FilePath));

            // start 2 tasks ( read doc. 1 && read doc. 2 )
            wordsOfDoc1.Start();
            wordsOfDoc2.Start();
            
            // calc magnitude1
            wordsOfDoc1.Wait();
            wordsOfDoc2.Wait();
            Task<double> magnitude1 = new Task<double>(() => CalcMagnitude(wordsOfDoc1.Result));
            

            // calc magnitude1
            
            Task<double> magnitude2 = new Task<double>(() => CalcMagnitude(wordsOfDoc2.Result));
            

            // calc dotProduct
            Task<double> dotProduct = new Task<double>(() => CalcDotProduct(wordsOfDoc1.Result,wordsOfDoc2.Result));
            dotProduct.Start();
            magnitude1.Start();
            magnitude2.Start();
            // wait ( calc magnitude1 && calc magnitude2 ) compleate to calc its sqrt
            magnitude1.Wait();
            magnitude2.Wait();

            // calc sqrt of ( magnitude1 && magnitude2 )
            Task<double> sqrtOfTwoMagnitudes = new Task<double>(() => CalcSqrtOfTwoMagnitudes(magnitude1.Result, magnitude2.Result));
            sqrtOfTwoMagnitudes.Start();

            // wait ( calc dorProduct && calc sqrt of ( magnitude1 && magnitude2 ) ) compleate to calc angle
            dotProduct.Wait();
            sqrtOfTwoMagnitudes.Wait();

            // calc angle
            return ((Math.Acos( dotProduct.Result / sqrtOfTwoMagnitudes.Result ))  * (180 / Math.PI));


        }
        


         // #############  Solution using Parallel (Threading)  ##############

        public static Dictionary<string, long> ReadDocuments(string docFilePath)
        {
            string Doc = File.ReadAllText(docFilePath).ToLower();
           
            // logic 1  ==> Average execution time (ms) = 603  &&  Max execution time (ms) = 1579
            //string [] words  = Doc.Split(new char[] {(char)65533,' ','!','(',')','-','_','[',']','/',':',';','\"','\'','<','>',',','.','?','\n','\r'},StringSplitOptions.RemoveEmptyEntries); // All special characters
           
            //(char)65533 (Symbol Name: Replacement Character),(char)12 (Symbol Name:form feed) ==> ASCII Code of non-alpanumeric characters , get it from func. calc that i created  


            // ,(char)12,'␌','\f','`','~','@','#','$','%','^','&','*','+','=','{','}','|','\\','a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z','0','1','2','3','4','5','6','7','8','9'
            //['!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '+', '=', '[', ']', '{', '}', '/', ', ', '.', '<','>','?'

            // logic 2  ==> Average execution time (ms) = 1092.9  &&  Max execution time (ms) = 2841
            string []words = Regex.Replace(Doc,@"[^A-Za-z0-9]"," ").Split(' ');


            char [] c = {'a','b'};
            Console.WriteLine(c['a']);
            Dictionary<string, long>  wordsOfDoc = new Dictionary<string, long>();

            foreach ( string word in words )
            {

                if(wordsOfDoc.ContainsKey(word)) // existing word
                {
                   // if(wordsOfDoc[word] < 100000) // NOTE: Max Freq. Value BOUNDED BY 100,000 from DR. A. salah
                    //{
                        wordsOfDoc[word] += 1;
                    //}
                }
                else if(!word.Equals(""))
                {
                     wordsOfDoc.Add(word, 1); // new word
                }
            }
             
            return wordsOfDoc;
        }

        public static double CalcDotProduct(Dictionary<string, long> doc1, Dictionary<string, long> doc2)
        {
            double dotProduct = 0.0;
            foreach (string word in doc1.Keys)
            {
                if (doc2.ContainsKey(word)) // check if word exist in two documents ==> else dotProduct = 0
                {
                    dotProduct += doc1[word] * doc2[word];
                }
            }
            return dotProduct;
        }

        public static double CalcMagnitude(Dictionary<string, long> doc)
        {
            double magnitude = 0.0;
            foreach (string word in doc.Keys)
            {
               //magnitude += Math.Pow(doc[word], 2);   // O(logBase2(n)
               magnitude += doc[word] * doc[word];      // O(1) ==> faster
            }
            return magnitude;
        }

        public static double CalcSqrtOfTwoMagnitudes(double magnitude1, double magnitude2)
        {
            return Math.Sqrt(magnitude1 * magnitude2);
        }

    }
}
