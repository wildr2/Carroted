using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MatchSetupPage : UIMenuPage
{
    public Transform camguide;
    public MainMenuPage page_mainmenu;
    public StartPopupPage start_popup_page;

    public Text[] txt_control, txt_color, txt_name;
    public InputField[] inputfield_name;
    public Text press_start_footer;
    public Text same_colors_footer;
    public Text txt_mem_time;

    public Player[] forshow_players;

    private GameSettings GS;

    private void Update()
    {
        UpdatePlayerJoin();
    }
    private void UpdatePlayerJoin()
    {
        bool input_start = false;
        int control_scheme = 0;
        if (Input.GetButtonDown("Start2"))
        {
            input_start = true;
            control_scheme = 2;
        }
        else if (Input.GetButtonDown("Start3")) 
        {
            input_start = true;
            control_scheme = 3;
        }


        if (input_start)
        {
            int player_num = GS.GetControlSchemePlayerNum(control_scheme);
            if (player_num == -1)
            {
                // new txt_control scheme wants to join
                if (GS.IsAIControlled(1))
                    GS.SetPlayerControl(1, false, control_scheme);
                else if (GS.IsAIControlled(2))
                    GS.SetPlayerControl(2, false, control_scheme);
            }
            else
            {
                // current player wants to leave
                GS.SetPlayerControl(player_num, true, 0);
            }

            // update control type button text
            txt_control[0].text = GS.GetControlTypeName(1);
            txt_control[1].text = GS.GetControlTypeName(2);

            // update footer
            UpdatePressStartFooter();
        }
    }
    protected override void OnStartTransitionIn()
    {
        GS = GameSettings.Instance;

        // camera
        Camera.main.transform.position = camguide.position;
        Camera.main.transform.rotation = camguide.rotation;

        // load settings
        LoadSettings();

        base.OnStartTransitionIn();
    }
    private void LoadSettings()
    {
        for (int i = 0; i < 2; ++i)
        {
            // Colors
            int id = GS.player_color_ID[i];
            txt_color[i].text = GS.player_color_names[id];
            txt_name[i].color = GS.GetPlayerColor(i+1, true);

            // Names
            inputfield_name[i].text = GS.player_name[i];         

            // Control type
            txt_control[i].text = GS.GetControlTypeName(i+1);

            // Misc
            txt_mem_time.text = GameSettings.Instance.GetMemorizeTimeMins() + " min to memorize";
        }

        // Footers
        UpdatePressStartFooter();
        UpdateSameColorsFooter();
    }
    private void UpdatePressStartFooter()
    {
        if (GS.IsAIControlled(1))
            press_start_footer.text = "Someone press start to play as " + inputfield_name[0].text;
        else if (GS.IsAIControlled(2))
            press_start_footer.text = "Someone press start to play as " + inputfield_name[1].text;
        else
            press_start_footer.text = "";
    }
    private void UpdateSameColorsFooter()
    {
        same_colors_footer.gameObject.SetActive(GS.PlayerSameColors());
    }
    private IEnumerator FlashSameColorFooter()
    {
        Color c = same_colors_footer.color;
        for (int i = 0; i < 5; ++i)
        {
            same_colors_footer.color = Color.clear;
            yield return new WaitForSeconds(0.1f);
            same_colors_footer.color = c;
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void OnButtonControl(int player_num)
    {
        if (GS.IsAIControlled(player_num))
        {
            // change ai type
            int n = GS.ai_names.Length;
            int type = GS.GetAIType(player_num);
            type = (type + 1) % n;
            GS.SetPlayerControl(player_num, true, type);
        }
        else
        {
            GS.SetPlayerControl(player_num, true, 0);
        }

        // update footer
        UpdatePressStartFooter();

        // update button text
        txt_control[player_num - 1].text = GS.GetControlTypeName(player_num);
    }
    public void OnButtonWeapon(int player_num)
    {

    }
    public void OnButtonColor(int player_num)
    {
        // change color
        int n = GS.player_colors.Length;
        int p = player_num - 1;
        int id = GS.player_color_ID[p];
        id = (id + 1) % n;
        GS.player_color_ID[p] = id;

        // update button color name
        txt_color[p].text = GS.player_color_names[id];

        // update player name color
        //   - use white if random selected
        txt_name[p].color = GS.GetPlayerColor(player_num, true);

        // same colors?
        UpdateSameColorsFooter();
    }
    public void OnNameChange(int player_number)
    {
        int p = player_number - 1;
        GS.player_name[p] = inputfield_name[p].text;

        // Footer
        UpdatePressStartFooter();
    }
    public void OnButtonMemTime()
    {
        // change time
        int n = GS.memorize_times.Length;
        GS.memorize_time_id = (GS.memorize_time_id + 1) % n;

        // update button
        txt_mem_time.text = GameSettings.Instance.GetMemorizeTimeMins() + " min to memorize";
    }

    public void OnButtonStart()
    {
        if (GS.PlayerSameColors())
        {
            StartCoroutine(FlashSameColorFooter());
            return;
        }
        start_popup_page.TransitionIn();
    }
    public void OnButtonBack()
    {
        TransitionOut();
        page_mainmenu.TransitionIn();
    }


    
}
