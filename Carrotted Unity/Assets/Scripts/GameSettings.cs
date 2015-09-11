using UnityEngine;
using System.Collections;


/// <summary>
/// Singleton containing settings (set in menus) for gameplay
/// </summary>
public class GameSettings : MonoBehaviour
{
    private static GameSettings _instance;
    public static GameSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameSettings>();

                if (_instance == null) Debug.LogError("Missing GameSettings");
                else
                {
                    DontDestroyOnLoad(_instance);
                    _instance.Initialize();
                }
            }
            return _instance;
        }
    }

    // Audio volumes
    public float volume_fx = 1, volume_music = 0.1f;

    // Player info
    private int[] player_control_scheme = { -1, -1 }; // -1 is ai...
    private int[] player_ai_type = { 0, 0 }; // -1 is human controlled
    public string[] player_name = { "Player 1", "Player 2" };
    public int[] player_color_ID = { 0, 0 };

	// Match info
    public int match_type;
    public bool music_on = true;

    // Constant data  (perhaps load from file in future)
    [System.NonSerialized]
    public Color[] player_colors; // set in initialize
    [System.NonSerialized]
    public string[] player_color_names = { "random color", "red", "pink", "purple", "blue",
           "teal", "green", "lime", "yellow", "orange", "maroon" };
    private string[] hex_colors = { "ffffff", "ff0000", "ff00c6", "8949ff", "4c6eff",
           "4cc2ff", "09ff49", "a5ff09", "f8ff38", "ff9a38", "6f1a4a" };
    [System.NonSerialized]
    public string[] ai_names = { "Easy AI", "Medium AI", "Hard AI" };


    // PUBLIC MODIFIERS

    public void SetPlayerControl(int player_number, bool ai, int type_or_scheme)
    {
        int p = player_number-1;
        if (ai)
        {
            player_control_scheme[p] = -1;
            player_ai_type[p] = type_or_scheme;
        }
        else
        {
            player_control_scheme[p] = type_or_scheme;
            player_ai_type[p] = -1;
        }
    }



    // PRIVATE MODIFIERS

    private void Awake()
    {
        // if this is the first instance, make this the singleton
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(_instance);
        }
        else
        {
            // destroy other instances that are not the already existing singleton
            if (this != _instance)
                Destroy(this.gameObject);
        }

        Initialize();
    }
    private void Initialize()
    {
        // colors
        InitializeColorsFromHex();

        // game startup random colors
        SetUnchosenPlayerColors();
    }
    private void InitializeColorsFromHex()
    {
        player_colors = new Color[hex_colors.Length];
        for (int i = 0; i < hex_colors.Length; ++i)
        {
            player_colors[i] = GeneralHelpers.HexToColor(hex_colors[i]);
        }
    }

    /// <summary>
    /// If any player has selected random color, this will choose a color for them, insuring that no players
    /// have the same color
    /// </summary>
    private void SetUnchosenPlayerColors()
    {
        for (int i = 0; i < 2; ++i)
        {
            if (player_color_ID[i] == 0)
            {
                player_color_ID[i] = Random.Range(1, player_colors.Length - 1);
                if (PlayerSameColors()) ++player_color_ID[i];
                if (player_color_ID[i] == 0) ++player_color_ID[i]; // can't random to random
            }
        }
    }


    // PUBLIC ACCESSORS

    /// <summary>
    /// If can be random is true and "random color" is currently selected, white is returned and
    /// the color is not changed.
    /// </summary>
    /// <param name="player_number"></param>
    /// <param name="can_be_random"></param>
    /// <returns></returns>
    public Color GetPlayerColor(int player_number, bool can_be_random)
    {
        GameSettings s = Instance;

        // set colors for players with random color selected
        if (s.player_color_ID[player_number - 1] == 0 && !can_be_random)
            s.SetUnchosenPlayerColors();

        return s.player_colors[s.player_color_ID[player_number - 1]];
    }
    public Color GetPlayerColor(int player_number)
    {
        return GetPlayerColor(player_number, false);
    }
    /// <summary>
    /// Returns whether the players have the same player color selected (random color doesn't count).
    /// </summary>
    /// <returns></returns>
    public bool PlayerSameColors()
    {
        GameSettings s = Instance;
        return s.player_color_ID[0] == s.player_color_ID[1] && s.player_color_ID[0] != 0;
    }    
    public bool IsAIControlled(int player_number)
    {
        return player_control_scheme[player_number - 1] == -1;
    }
    public int GetAIType(int player_number)
    {
        return player_ai_type[player_number - 1];
    }
    /// <summary>
    /// Reuires: specified player must not be ai controlled
    /// </summary>
    /// <param name="player_number"></param>
    /// <returns></returns>
    public int GetHumanControlScheme(int player_number)
    {
        return player_control_scheme[player_number - 1];
    }
    public string GetControlTypeName(int player_number)
    {
        if (!IsAIControlled(player_number)) return "Human";
        return ai_names[player_ai_type[player_number - 1]];
    }
    /// <summary>
    /// Returns -1 if this control scheme has no associated player
    /// </summary>
    /// <param name="control_scheme"></param>
    /// <returns></returns>
    public int GetControlSchemePlayerNum(int control_scheme)
    {
        return player_control_scheme[0] == control_scheme ? 1 :
            player_control_scheme[1] == control_scheme ? 2 : -1;
    }
}
