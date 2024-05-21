using System;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Windows.Forms;

namespace CasinoV3
{
    public partial class Form1 : Form
    {
        private Random random = new Random();
        private string[] symbols = { "🍒", "🍋", "🍉", "🍇", "🔔" };
        private Label[,] symbolLabels = new Label[3, 3];
        private bool spinning = false;
        private int tickCount = 0;
        private int stopAfterTicks = 7;


        private double balance = 1000.00;
        private double bet = 0.00;
        private double multiplier = 0.00;
        private double winAmount = 0.00;



        //rtp section
        private int rtp = 95;
        private bool needWin = false;
        private int spinCounter = 0;
        private double pastBalance = 1000.00;
        

        public Form1()
        {
            InitializeComponent();


            UpdateBalanceDisplay();

            InitializeSymbolLabels();
            timer1.Interval = 100;
            timer1.Tick += timer1_Tick;
        }

        private void InitializeSymbolLabels()
        {
            int labelIndex = 1;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    string labelName = $"label{labelIndex++}";
                    symbolLabels[i, j] = this.Controls.Find(labelName, true).FirstOrDefault() as Label;
                    if (symbolLabels[i, j] == null)
                    {
                        MessageBox.Show($"Label not found: {labelName}");
                        return;
                    }
                }
            }
        }

        

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (tickCount < stopAfterTicks)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        symbolLabels[i, j].Text = symbols[random.Next(symbols.Length)];
                    }
                }
                tickCount++;
            }
            else if (needWin)
            {
                //spin variation
                /*
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        symbolLabels[i, j].Text = symbols[random.Next(symbols.Length)];


                        bool allSame = symbolLabels.Cast<Label>().Where((label, index) => index >= 3 && index < 6)
                        .Select(label => label.Text)
                        .Distinct()
                        .Count() == 1;

                        if (allSame)
                        {
                            needWin = false;
                            break;
                        }

                    }
                

                }
                
                 */

                int randomSymbol = random.Next(symbols.Length);

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (i == 1)
                        {
                            symbolLabels[i, j].Text = symbols[randomSymbol];
                        }
                        else
                        {
                            symbolLabels[i, j].Text = symbols[random.Next(symbols.Length)];
                        }
                        
                    }
                }
                needWin = false;
                pastBalance = balance;

                //tickCount++;
            }
            else
            {
                timer1.Stop();
                spinning = false;
                CheckWin();
            }
        }

        private void CheckWin()
        {
            bool allSame = symbolLabels.Cast<Label>().Where((label, index) => index >= 3 && index < 6)
                .Select(label => label.Text)
                .Distinct()
                .Count() == 1;

            if (allSame)
            {
                string uniqueSymbol = symbolLabels.Cast<Label>().Where((label, index) => index >= 3 && index < 6)
                .Select(label => label.Text)
                .Distinct()
                .First();

                switch (uniqueSymbol)
                {
                    case "🍉": 
                        multiplier = 3.00; 
                        break;

                    case "🍒":
                        multiplier = 5.00;
                        break;

                    case "🍇":
                        multiplier = 10.00;
                        break;

                    case "🍋":
                        multiplier = 15.00;
                        break;

                    case "🔔":
                        multiplier = 30.00;
                        break;
                }

                winAmount = bet * multiplier;

                balance = winAmount + balance;
                UpdateBalanceDisplay();

                MessageBox.Show($"Win!\n{winAmount}$");
            }

        }



        private void UpdateBalanceDisplay()
        {
            labelBalance.Text = $"Balance: ${balance}";
        }




        private void button1_Click_1(object sender, EventArgs e)
        {
            if (!spinning)
            {
                string selectedBet = comboBoxBet.SelectedItem.ToString();
                selectedBet = selectedBet.Substring(0, selectedBet.Length - 1);

                bet = Convert.ToDouble(selectedBet);

                balance = balance - bet;
                UpdateBalanceDisplay();


                spinning = true;
                
                needWin = RtpChecker();
                tickCount = 0;
                timer1.Start();
            }
            else
            {

            }
            
        }


        private bool RtpChecker()
        {
            int randomSpin = random.Next(10, 21);

            double balanceNeeded = pastBalance / 100 * rtp; 

            if (balance < balanceNeeded)
            {
                spinCounter++;
            }

            if (spinCounter >= randomSpin && balance < balanceNeeded )
            {
                needWin = true;
                spinCounter = 0;
            }

            
            
            return needWin;
        }






    }
}
