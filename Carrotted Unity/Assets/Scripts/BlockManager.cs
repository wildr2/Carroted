using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;
using System.IO;

public class BlockManager : MonoBehaviour
{
    public bool debug = false;

    // Blocks
    public Block block_prefab;
    
    private List<Block> blocks;
    private Block read_block;

    public Transform corner_bottomleft, corner_topright;
    private int rows = 4, cols = 8;
    private float mid_gap = 2;

    // Words & phrases
    private List<string> phrases;
    private List<string> words;
    private static List<char> phrase_seperators = new List<char> { ',', '.', '?', '!', ':', ';' };

    
    // Reading
    public TextMesh reader;
    private bool reading = false;
    private int[] read_order;
    private int read_order_i = 0;
    private float read_speed = 10f; // chars per second

    private string current_phrase;
    private string current_word;
    private string current_written_word; // the word being typed out now
    private int read_letter_i = 0;

    public Action event_read_done;


    // Audio
    private ReaderAudio reader_audio;



    // PRIVATE MODIFIERS

    private void Start()
    {
        reader_audio = GetComponentInChildren<ReaderAudio>();
    }
    private void GenerateReadOrder()
    {
        read_order = Enumerable.Range(0, GetNumBlocks()).ToArray();
        GeneralHelpers.ShuffleArray(read_order);
    }
    private void CreateBlocks()
    {
        float w = corner_topright.position.x - corner_bottomleft.position.x;
        float h = corner_topright.position.z - corner_bottomleft.position.z;
        float w_step = (w - mid_gap) / (cols - 1);
        float h_step = h / (rows - 1);

        blocks = new List<Block>();

        int i = 0;
        for (int x = 0; x < cols; ++x)
        {
            for (int y = 0; y < rows; ++y)
            {
                // Create block
                Block b = Instantiate(block_prefab);
                float offset = x >= cols / 2 ? mid_gap : 0;
                Vector3 pos = new Vector3(corner_bottomleft.position.x + x * w_step + offset,
                                          corner_bottomleft.position.y,
                                          corner_bottomleft.position.z + y * h_step);
                b.transform.parent = transform;

                b.Initialize(words[i], pos);
                blocks.Add(b);

                ++i;
            }
        }
    }
    private void ChooseWords()
    {
        int n = GetNumBlocks();
        phrases = new List<string>();
        words = new List<string>();

        string alltext = File.ReadAllText("Assets/Texts/Book2.txt");
       
        
        int attempts = 0;
        while (attempts < 5)
        {
            // find n phrases
            phrases = new List<string>();
            for (int i = 0; i < n; ++i)
            {
                string p = "";
                do 
                {
                    p = FindPhrase(alltext);
                } while (p.Length > 90);
                phrases.Add(p);
            }

            // find words
            FindUniqueWords();

            // check whether unique words were found for each phrase
            bool allgood = true;
            for (int i = 0; i < n; ++i)
            {
                if (words[i] == "")
                {
                    allgood = false;
                    break;
                }
            }
            if (allgood) break;
            attempts++;
        }
    }
    private void ChooseWordsDebug()
    {
        phrases = new List<string>();
        words = new List<string>();

        for (int i = 0; i < GetNumBlocks(); ++i)
        {
            phrases.Add("contains a word");
            words.Add("word");
        }
    }


    private void FindUniqueWords()
    {
        int n = GetNumBlocks();

        // find word frequencies (unique or no)
        Dictionary<string, int> seen_words = new Dictionary<string, int>();
        for (int i = 0; i < n; ++i)
        {
            foreach (string word in phrases[i].Split(new char[] { ' ', '\t', '\r', '\n' }))
            {
                if (seen_words.ContainsKey(word)) seen_words[word] = -1;
                else seen_words[word] = i;
            }
        }

        // init word list
        for (int i = 0; i < n; ++i) words.Add("");

        // find and save unique words
        foreach (KeyValuePair<string, int> kv in seen_words)
        {
            if (kv.Value != -1) words[kv.Value] = kv.Key;
        }
    }
    private bool IsGoodWord(string word)
    {
        return word.Length >= 3 && !words.Contains(word);
    }
    private string FindWord(string phrase)
    {
        string word = "";
        string[] phrase_words = phrase.Split(new char[] { ' ', '\t', '\r', '\n' });

        // choose random word 
        int word_index = UnityEngine.Random.Range(0, phrase_words.Length);

        while (true)
        {
            word = phrase_words[word_index];
            if (IsGoodWord(word)) return word;
            else
            {
                word_index++;
                if (word_index >= phrase_words.Length)
                {
                    // bad phrase
                    return "";
                }
            }
        }
    }
    private string FindPhrase(string alltext)
    {
        string phrase = "";

        int j = UnityEngine.Random.Range(0, alltext.Length);
        while (!phrase_seperators.Contains(alltext[j]))
        {
            ++j;
        }
        while (phrase_seperators.Contains(alltext[j]))
        {
            ++j;
        }

        while (true)
        {
            phrase += alltext[j];
            ++j;
            if (phrase_seperators.Contains(alltext[j])) break;
        }

        // cleanup the phrase
        phrase = phrase.Replace("'", "");
        phrase = phrase.Replace("\"", "");
        phrase = phrase.Replace("\n", "");
        phrase = phrase.Replace("\r\n", "");
        phrase = phrase.Replace("\r", "");
        phrase = phrase.Replace("\t", "");
        phrase = phrase.Replace("  ", " ");
        phrase = phrase.Trim();
        phrase = phrase.ToLower();

        return phrase;
    }

    private IEnumerator UpdateRead()
    {
        string[] words = current_phrase.Split();
        current_written_word = words[0];

        reading = true;
        read_letter_i = 0;
        int word_i = 0;
        reader_audio.PlayReading();
        reader_audio.PlayReadWord(current_written_word.Length);

        while (read_letter_i < current_phrase.Length)
        {
            reader.text += current_phrase[read_letter_i];
            ++read_letter_i;

            if (word_i < words.Length - 1 && current_phrase[read_letter_i] == ' ')
            {
                ++word_i;
                current_written_word = words[word_i];
                reader_audio.PlayReadWord(current_written_word.Length);
            }

            yield return new WaitForSeconds(1/read_speed);
        }
        reading = false;
        if (event_read_done != null) event_read_done();

        reader_audio.StopReading();
    }


    // PUBLIC MODIFIERS

    public void Initialize()
    {
        if (debug) ChooseWordsDebug();
        else ChooseWords();
        CreateBlocks();
        GenerateReadOrder();
    }
    public void ReadNext()
    {
        // get next phrase and word
        int i = read_order[read_order_i];
        current_phrase = phrases[i];
        current_word = words[i];
        read_block = blocks[i];
        read_order_i++;

        reader.text = "";
        StartCoroutine(UpdateRead());
    }
    public void ResetBlocks()
    {
        foreach (Block b in blocks)
        {
            if (b != null) b.Reset();
        }
    }
    public void ResetReader()
    {
        reader.text = "...";
    }


    // PUBLIC ACCESSORS

    
    public int GetNumBlocks()
    {
        return rows * cols;
    }
    public bool IsReading()
    {
        return reading;
    }
    public Block GetReadBlock()
    {
        return read_block;
    }
    public bool IsWritingReadWord()
    {
        return current_written_word.Trim() == current_word;
    }
    public string GetReadWord()
    {
        return current_word;
    }
    public string GetReadPhrase()
    {
        return current_phrase;
    }
    public int GetReadLetterIndex()
    {
        return read_letter_i;
    }
    public int UnreadBlocks()
    {
        return blocks.Count - read_order_i;
    }
}
