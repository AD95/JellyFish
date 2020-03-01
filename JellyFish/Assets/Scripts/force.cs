﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;


public class force : MonoBehaviour
{
    public static int health = 3;

    public static string actual_word = "";
    public static string hint_1 = "";
    public static string hint_2 = "";
    public static string hint_3 = "";
    public static string category = "";

    int direction = -1;

    public static char[] allCharacters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

    public GameObject h1, h2, h3, gameover;
    // the three hearts

    //public hints_panel hp;

    // Update is called once per frame
    public static string word1=actual_word.ToUpper();
    public static string word_formed = new string(word1.Distinct().ToArray());

    hints_panel hp;
    //public static string word_formed = word1;
    public int word_length = word_formed.Length;
    public static char[] char_arr = word_formed.ToCharArray();
    public Text wordCreated;
    static public int i = 0;
    public static List<char> CorrectandIncorrect = new List<char>();
    static char[] remaining;

    public string getHint()
    {
        Debug.Log(hint_1 + "\n" + hint_2 + "\n" + hint_3);
        return hint_1 +"\n" + hint_2 +"\n"+ hint_3;
    }

    public string getCat()
    {
        Debug.Log(category);
        return category;
    }

    class CSVReader
    {
        static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
        static char[] TRIM_CHARS = { '\"' };

        public static List<Dictionary<string, object>> Read(string file)
        {
            var list = new List<Dictionary<string, object>>();
            TextAsset data = Resources.Load(file) as TextAsset;

            var lines = Regex.Split(data.text, LINE_SPLIT_RE);

            if (lines.Length <= 1) return list;

            var header = Regex.Split(lines[0], SPLIT_RE);
            for (var i = 1; i < lines.Length; i++)
            {

                var values = Regex.Split(lines[i], SPLIT_RE);
                if (values.Length == 0 || values[0] == "") continue;

                var entry = new Dictionary<string, object>();
                for (var j = 0; j < header.Length && j < values.Length; j++)
                {
                    string value = values[j];
                    value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                    object finalvalue = value;
                    int n;
                    float f;
                    if (int.TryParse(value, out n))
                    {
                        finalvalue = n;
                    }
                    else if (float.TryParse(value, out f))
                    {
                        finalvalue = f;
                    }
                    entry[header[j]] = finalvalue;
                }
                list.Add(entry);
            }
            return list;
        }
    }




    //static public char[] char_arr1 = "brunda".ToCharArray();

    void Update()
    {
        //hp = gameObject.AddComponent<hints_panel>();
        //Debug.Log(hp.getPaused());
        if (hp != null && !hp.getPaused())
        {
            transform.Translate(Time.deltaTime * 2 * direction, 0, 0);
        }
        
    }
    void Start()
    {

        Debug.Log("1");
        //hp.paused = false;
        h1 = GameObject.FindGameObjectWithTag("heart1");
        h2 = GameObject.FindGameObjectWithTag("heart2");
        h3 = GameObject.FindGameObjectWithTag("heart3");
        // game object reference put using unity ui only
        Debug.Log("2");

        if (remaining == null)
        {
            Debug.Log("21");

            List<Dictionary<string, object>> data = CSVReader.Read("data");
            var random_index_1 = Random.Range(0, data.Count);
            Debug.Log("3");

            actual_word = data[random_index_1]["word"].ToString();
            hint_1 = data[random_index_1]["Hint 1"].ToString();
            hint_2 = data[random_index_1]["Hint 2"].ToString();
            hint_3 = data[random_index_1]["Hint 3"].ToString();
            category = data[random_index_1]["Category"].ToString();
            Debug.Log("4");

            word1 = actual_word.ToUpper();
            word_formed = new string(word1.Distinct().ToArray());
            word_length = word_formed.Length;
            char_arr = word_formed.ToCharArray();
            remaining = new string(allCharacters.Except(char_arr).ToArray()).ToCharArray();
            Debug.Log("5");

            for (int j = 0; j < word_length + 3; j++)
            {
                if (j >= word_length)
                {
                    var random_index = Random.Range(0, remaining.Length);
                    CorrectandIncorrect.Add(remaining[random_index]);
                }
                else
                {
                    CorrectandIncorrect.Add(char_arr[j]);
                }
            }
            Debug.Log(new string(remaining.ToArray()));
        }
        Text t = gameObject.GetComponentInChildren<Text>();
        Debug.Log(t.text);
        //if (i < char_arr.Length)
        Debug.Log(CorrectandIncorrect.Count);
        //if (i < CorrectandIncorrect.Count)
        {
            //var random_index = Random.Range(0, char_arr.Length);
            var random_index = Random.Range(0, CorrectandIncorrect.Count);
            t.text = CorrectandIncorrect[random_index].ToString();
           // i++;

        }
        //else if (i == char_arr.Length)
       // else if (i == CorrectandIncorrect.Count)
        {
         //   i = 0;
        }

        hp = GameObject.FindGameObjectWithTag("hint_button").GetComponent<hints_panel>();


    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hp!=null && hp.getPaused())
            return;
        Debug.Log(word_formed);
		if (collision.gameObject.tag == "bullet")
		{
            //GameObject g = Instantiate(collision.gameObject, new Vector3(0,0), transform.rotation);
            collision.gameObject.SetActive(false);
            gameObject.SetActive(false);
            Invoke("ResetBall", 0.2f);
            Text t = gameObject.GetComponentInChildren<Text>();

            if (word_formed.Contains(t.text[0]) && CorrectandIncorrect.Contains(t.text[0])) //Condition when it is a correct character  
            {
                var foundIndexes = new List<int>();
                

                for (int j = word_formed.IndexOf(t.text[0]); j > -1; j = word_formed.IndexOf(t.text[0], j+ 1))
                {
                    // for loop end when i=-1 ('a' not found)
                    foundIndexes.Add(j);
                }
                char[] x = wordCreated.text.ToCharArray();
                Debug.Log(x.ToString());
                for (int j = 0; j < foundIndexes.Count; j++)
                {
                    x[foundIndexes[j] * 2]= t.text[0];
   

                }
                Debug.Log("Before collision" + new string(CorrectandIncorrect.ToArray()));
                CorrectandIncorrect.Remove(t.text[0]);
                Debug.Log("After collision" + new string(CorrectandIncorrect.ToArray()));
                int random_index;
                do { random_index = Random.Range(0, remaining.Length);
                }
                while (CorrectandIncorrect.Contains(remaining[random_index]));
                CorrectandIncorrect.Add(remaining[random_index]);

                wordCreated.text =new string(x);
                if (wordCreated.text.Replace(" ", "").Equals(word_formed))
                {
                    Text t11 = gameover.GetComponentInChildren<Text>();
                    t11.text = "Yay!";
                    gameover.SetActive(true);
                    hp.setPaused(true);
                    hp.gameObject.SetActive(false);
                    gameObject.SetActive(false);
                    collision.gameObject.SetActive(false);
                }
            }


            else {
            /** decrement health **/
            health--;

            if (health == 2)
            {
                h1.SetActive(false);
            }

            else if(health==1){
                h2.SetActive(false);
            }
            else if (health == 0)
                {
                    Text t11 = gameover.GetComponentInChildren<Text>();
                    t11.text = "Game Over!";
                    gameover.SetActive(true);
                    hp.setPaused(true);
                    hp.gameObject.SetActive(false);
                    gameObject.SetActive(false);
                    collision.gameObject.SetActive(false);
                    h3.SetActive(false);
                    
                }
            }
	     if (i < CorrectandIncorrect.Count) {
                var random_index = Random.Range(0, CorrectandIncorrect.Count);
                
                t.text = CorrectandIncorrect[random_index].ToString();
                //t.text = "B";
                i++;
                
            }
            else if (i == CorrectandIncorrect.Count)
            {
                
                i = 0;
            }


            
        }
        else if(collision.gameObject.tag == "SideWall")
		{
			direction *= -1;
		}
        
    }
    public void ResetBall()
    {
        gameObject.SetActive(true);
        direction *= -1;
    }
}
