using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;


public class GameManagement : MonoBehaviour
{
    // Variables
    // // UI
    [Header("UI Documents")]
    public UIDocument gameUI;
    public UIDocument menuUI;
    public UIDocument weekendUI;
    
    private Button _btnShop;
    private Button _btnStartWeek;
    private Button _btnGameMenu;
    private Label _lblCurrMoney;
    private Label _lblDayName;
    private Label _lblWeekNum;

    private GroupBox _boxWeekEnd;
    private Label _lblweekmessage;
    private Label _lblcustserved;
    private Label _lblcoffeebagsused;
    private Label _lblmilkjugsbagsused;
    private Label _lblsuppliescost;
    private Label _lblstartingbalance;
    private Label _lblrentcost;
    private Label _lblweekprofit;
    private Label _lblclosingbalance;
    private Button _btnCloseweek;

    private GroupBox _boxSettings;
    private Button _btnResetGame;
    private Button _btnCloseSettings;

    private GroupBox _boxUpgrades;
    private Label _lblShopWarning;
    private Button _btnUpgradeCoffee;
    private Button _btnUpgradeMilk;
    private Button _btnUpgradeCustomer;
    private Button _btnCloseShop;

    private bool _gamemenuvisible;
    private bool _shopmenuvisible;
    
    // // Time Controls
    private int _weeknum;
    private int _daynum;
    private List<string> _dayname;
    
    // // Animations
    private int _sunticker;
    public GameObject theSun;
    private List<Vector2> _sunLocations;

    // // Finances
    private float _currentMoney;
    private float _rentCost;
    private float _cupCost;
    private float _weekstartingmoney;
    
    // // Simulation Numbers
    private float _customerinput;
    private float _customerinputlow;
    private float _customerinputhigh;
    private int _coffeemultiplier;
    private int _milkmultiplier;
    private float _weekrevenue;
    private float _coffeeused;
    private float _coffeeusecost;
    private float _milkused;
    private float _milkusecost;
    private float _supplycost;
    private float _weekprofit;
    private float _endingweekcash;
    private float _coffeeperbag;
    private float _coffeebagcost;
    private float _milkperjug;
    private float _milkjugcost;
    private float _customers;

    // Upgrade Tracking
    private int _coffeeupgradenum;
    private int _milkupgradenum;
    private int _customerupgradenum;
    private List<int> _upgradecosts;

    // Start is called before the first frame update
    void Start()
    {
        // Establish Variables
        // // Time Tracking
        _weeknum = 0;
        _daynum = 1;
        _dayname = new List<string>() { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
        
        
        // // Finances
        _currentMoney = 50;
        _rentCost = 500;
        _cupCost = 3;
        _weekstartingmoney = 0;  
        
        // // Simulation Numbers
        _customerinput = 1;
        _customerinputlow = 1;
        _customerinputhigh = 1.5f;
        _coffeemultiplier = 20;
        _milkmultiplier = 20;
        
        // // Resources
        _coffeeperbag = .28f;
        _milkperjug = .25f;
        _coffeebagcost = 20;
        _milkjugcost = 10;
        
        // Upgraade Starting Values
        _coffeeupgradenum = 0;
        _milkupgradenum = 0;
        _customerupgradenum = 0;
        _upgradecosts = new List<int>() {999,2000,3000,4000,8000,16000,36000,72000,99999};
        
        // // animations
        _sunticker = 0;
        _sunLocations = new List<Vector2>()
        {
            new Vector2(-3,.6f),
            new Vector2(-2,1f),
            new Vector2(-1,1.5f),
            new Vector2(1,2),
            new Vector2(2,1.5f),
            new Vector2(3,1),
            new Vector2(4,0)
        };
        
        
        
        // Establish UI Docs
        VisualElement gameuiRoot = gameUI.rootVisualElement;
        VisualElement weekuiRoot = weekendUI.rootVisualElement;
        VisualElement menuuiRoot = menuUI.rootVisualElement;
        
        // Query UI Elements
        // // Game UI
        _btnShop = gameuiRoot.Q<Button>("btn_shop");
        _btnStartWeek = gameuiRoot.Q<Button>("btn_start_week");
        _btnGameMenu = gameuiRoot.Q<Button>("btn_game_menu");
        _lblCurrMoney = gameuiRoot.Q<Label>("lbl_curr_money");
        _lblDayName = gameuiRoot.Q<Label>("lbl_day_name");
        _lblWeekNum = gameuiRoot.Q<Label>("lbl_week_num");
        
        // // Week End UI
        _boxWeekEnd = weekuiRoot.Q<GroupBox>("box_week_end");
        _lblweekmessage = weekuiRoot.Q<Label>("lbl_week_num_message");
        _lblcustserved = weekuiRoot.Q<Label>("lbl_cust_served");
        _lblcoffeebagsused = weekuiRoot.Q<Label>("lbl_coffee_bags_used");
        _lblmilkjugsbagsused = weekuiRoot.Q<Label>("lbl_milk_jugs_used");
        _lblsuppliescost = weekuiRoot.Q<Label>("lbl_supplies_cost");
        _lblstartingbalance = weekuiRoot.Q<Label>("lbl_starting_cash");
        _lblrentcost = weekuiRoot.Q<Label>("lbl_rent_cost");
        _lblweekprofit = weekuiRoot.Q<Label>("lbl_week_profit");
        _lblclosingbalance = weekuiRoot.Q<Label>("lbl_closing_bal");
        _btnCloseweek = weekuiRoot.Q<Button>("btn_close_week_popup");
        
        // // Menu UI
        _boxSettings = menuuiRoot.Q<GroupBox>("box_settings");
        _btnResetGame = menuuiRoot.Q<Button>("btn_reset_game");
        _btnCloseSettings = menuuiRoot.Q<Button>("btn_settings_close");
        
        _boxUpgrades = menuuiRoot.Q<GroupBox>("box_shop");
        _lblShopWarning = menuuiRoot.Q<Label>("lbl_shop_warning");
        _btnUpgradeCoffee = menuuiRoot.Q<Button>("btn_coffee_up");
        _btnUpgradeMilk = menuuiRoot.Q<Button>("btn_milk_up");
        _btnUpgradeCustomer = menuuiRoot.Q<Button>("btn_customer_up");
        _btnCloseShop = menuuiRoot.Q<Button>("btn_close_shop");
        
        // UI Defaults
        _boxWeekEnd.visible = false;
        _boxSettings.visible = false;
        _boxUpgrades.visible = false;
        _lblShopWarning.visible = false;

        _lblDayName.text = "Monday";
        
        _btnUpgradeCoffee.text = "Upgrade Coffee $"+_upgradecosts[_coffeeupgradenum];
        _btnUpgradeMilk.text = "Upgrade Milk $"+_upgradecosts[_milkupgradenum];
        _btnUpgradeCustomer.text = "Upgrade Customer $"+_upgradecosts[_customerupgradenum];

            // Register Button Clicks
            // // Main Three Buttons
            _btnShop.clickable.clicked += () =>
            {
                Debug.Log("Shop Button Clicked");
                _shopmenuvisible ^= true;
            };
            _btnStartWeek.clickable.clicked += () =>
            {
                Debug.Log("Start Week Button Clicked");
                _daynum = 1;
                StartWeek();
            };
            _btnGameMenu.clickable.clicked += () =>
            {
                Debug.Log("Game Menu Clicked");
                _gamemenuvisible ^= true;
            };
            // // Week UI
            _btnCloseweek.clickable.clicked += () =>
            {
                Debug.Log("Weekend Close Button Clicked");
                _boxWeekEnd.visible = false;
                _btnStartWeek.visible = true;
            };
            // // Seetings Menu
            _btnCloseSettings.clickable.clicked += () =>
            {
                Debug.Log("Settings Close Button Clicked");
                _gamemenuvisible = false;

            };

            _btnResetGame.clickable.clicked += () =>
            {
                Debug.Log("Reset Game Button Clicked");
                ResetGameState();
                _boxSettings.visible = false;
            };
            // // Upgrade Shop Menu
            _btnCloseShop.clickable.clicked += () =>
            {
                Debug.Log("Shop Menu Close button Clicked");
                _shopmenuvisible = false;
                _lblShopWarning.visible = false;

            };

            _btnUpgradeCoffee.clickable.clicked += () =>
            {
                Debug.Log("Coffee Upgrade Button Clicked");
                if (_currentMoney < _upgradecosts[_coffeeupgradenum])
                {
                    _lblShopWarning.visible = true;
                }
                if (_currentMoney >= _upgradecosts[_coffeeupgradenum])
                {
                    var newcoffeemultiplier = _coffeemultiplier + 5;
                    _coffeemultiplier = newcoffeemultiplier;

                    var newcurrmoney = _currentMoney - _upgradecosts[_coffeeupgradenum];
                    _currentMoney = newcurrmoney;

                    _coffeeupgradenum++;
                    _btnUpgradeCoffee.text = "Upgrade Coffee $"+_upgradecosts[_coffeeupgradenum];
                }
                
            };
            
            _btnUpgradeMilk.clickable.clicked += () =>
            {
                Debug.Log("Milk Upgrade Button Clicked");
                if (_currentMoney < _upgradecosts[_milkupgradenum])
                {
                    _lblShopWarning.visible = true;
                }
                if (_currentMoney >= _upgradecosts[_milkupgradenum])
                {
                    var newmilkmulti = _milkmultiplier + 5;
                    _milkmultiplier = newmilkmulti;

                    var newcurrmoney = _currentMoney - _upgradecosts[_milkupgradenum];
                    _currentMoney = newcurrmoney;

                    _milkupgradenum++;
                    _btnUpgradeMilk.text = "Upgrade Milk $"+_upgradecosts[_milkupgradenum];
                }
            };

            _btnUpgradeCustomer.clickable.clicked += () =>
            {
                Debug.Log("Customer Upgrade Button Clicked");
                if (_currentMoney < _upgradecosts[_customerupgradenum])
                {
                    _lblShopWarning.visible = true;
                }
                if (_currentMoney >= _upgradecosts[_customerupgradenum])
                {
                    var newcustomermultilow = _customerinputlow + .3f;
                    _customerinputlow = newcustomermultilow;
                    
                    var newcustomermultihigh = _customerinputhigh + .3f;
                    _customerinputhigh = newcustomermultihigh;
                    
                    _customerupgradenum++;
                    _btnUpgradeCustomer.text = "Upgrade Customer $"+_upgradecosts[_customerupgradenum];
                }

                
            };


    }

    private void Update()
    {
        // Keep var constant
        _lblCurrMoney.text = "$" +_currentMoney.ToString(CultureInfo.CurrentCulture);
        
        _lblWeekNum.text = "Week Number: "+_weeknum.ToString();
       
        if(_gamemenuvisible == true){_boxSettings.visible = true;}
        if(_gamemenuvisible == false){_boxSettings.visible = false;}
        
        if(_shopmenuvisible == true){_boxUpgrades.visible = true;}
        if(_shopmenuvisible == false){_boxUpgrades.visible = false;}

        theSun.transform.position = new Vector2(_sunLocations[_sunticker].x,_sunLocations[_sunticker].y);

    }

    void StartWeek()
    {
        Debug.Log("Week Starting");
        _btnStartWeek.visible = false;
        _boxSettings.visible = false;
        _boxUpgrades.visible = false;
        _weekstartingmoney = _currentMoney;
        _customerinput = Random.Range(1, 1.5f);
        _weeknum++;
        
        StartCoroutine(BrewingCoffee());
    }

    private IEnumerator BrewingCoffee()
    {
        Debug.Log("Brewing Coffee");

        for (int i = 1; i < 8; i ++)
        {
            yield return new WaitForSeconds(1);
            if (i < 7)
            {
                Debug.Log("Todays Day is:"+_dayname[i]);
                _lblDayName.text = _dayname[i];
                _sunticker++;
            }

            if (i == 7)
            {
                
                Debug.Log("Todays Day is:"+_dayname[0]);
                _lblDayName.text = _dayname[0];
                EndWeek();
            }
            
            
        }
    }

    void EndWeek()
    {
        Debug.Log("Week Ending");
        //
        _customers = math.floor((_customerinput * _coffeemultiplier) + (_customerinput + _milkmultiplier));
        Debug.Log("Customers Served: " + _customers);
        _weekrevenue = (_customers*7)*_cupCost;
        Debug.Log("Revenue: " + _weekrevenue);
  
        //
        _coffeeused =  math.ceil((_customerinput * _coffeemultiplier) * _coffeeperbag);
        _coffeeusecost = _coffeeused * _coffeebagcost;
        Debug.Log("Coffee Bags Used: "+_coffeeused + " Cost: $" + _coffeeusecost);
        //
        _milkused = math.ceil((_customerinput * _milkmultiplier) * _milkperjug);
        _milkusecost = _milkused * _milkjugcost;
        Debug.Log("Milk Jugs Used: " + _milkused + " Cost: $:" + _milkusecost);
        //
        _supplycost = _milkusecost + _coffeeusecost;
        Debug.Log("Supplies this week cost:"+_supplycost);
        //
        _weekprofit = _weekrevenue - _supplycost;
        Debug.Log("Week Profit Cost: $" + _weekprofit);
        //
        _endingweekcash = _weekprofit - _rentCost;
        Debug.Log("Profit minus Rent =" + _endingweekcash);
        _currentMoney = _weekstartingmoney + _endingweekcash;

        _boxWeekEnd.visible = true;
        _lblweekmessage.text = "Week " + _weeknum + " has ended.";
        _lblcustserved.text = "Customers Served: "+_customers;
        _lblcoffeebagsused.text = "Coffee Bags: " + _coffeeused;
        _lblmilkjugsbagsused.text = "Milk Jugs: " + _milkused;
        _lblstartingbalance.text = "Week Starting Balance: $" + _weekstartingmoney;
        _lblrentcost.text = "Rent Cost: $"+ _rentCost;
        _lblsuppliescost.text = "Supplies Cost : $" + _supplycost;
        _lblweekprofit.text = "Week Profit: $" + _weekprofit;
        _lblclosingbalance.text = "Closing Balance: $" + _currentMoney;

        _sunticker = 0;

    }

    void ResetGameState()
    {
        // Reset all Values back to their default status to start the game again
        // // Time Tracking
        _weeknum = 0;
        _daynum = 1;

        // // Finances
        _currentMoney = 50;
        _rentCost = 500;
        _cupCost = 3;
        _weekstartingmoney = 0;
        
        // // Simulation Numbers
        _customerinput = 1;
        _coffeemultiplier = 20;
        _milkmultiplier = 20;
        
        // // Resources
        _coffeeperbag = .28f;
        _milkperjug = .25f;
        _coffeebagcost = 20;
        _milkjugcost = 10;
    }
    
}
