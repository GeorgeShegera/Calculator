using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace Calculator
{
    public partial class CalculatorForm : Form
    {        
        private double result = 0;

        private Operation curOperation = Operation.Sum;

        private bool displayResult = true;

        private bool exception = false;
        public CalculatorForm()
        {
            InitializeComponent();
        }
        private void numberBtn_Click(object sender, EventArgs e)
        {
            bool floatingDot = lbDisplay.Text.Any(x => x == ',');
            int lbTextCount = lbDisplay.Text.Count();
            if (exception) btnClear_Click(sender, e);
            string senderText = ((Button)sender).Text;
            if ((floatingDot && senderText == "," && !displayResult) ||
                ((!floatingDot && lbTextCount == 15) || lbTextCount == 16) && !displayResult)
            {
                SystemSounds.Beep.Play();
                return;
            }
            if (displayResult || lbDisplay.Text == "0")
            {
                if (senderText == ",") senderText = "0,";
                lbDisplay.Text = senderText;
            }
            else lbDisplay.Text += senderText;
            displayResult = false;
        }

        private void operationBtn_Click(object sender, EventArgs e)
        {
            string operation = ((Button)sender).Text;
            if (displayResult)
            {
                curOperation = ConvertOp(operation);
                if (history.Text.Length > 0) history.Text = history.Text.Remove(history.Text.Length - 1);
                else history.Text += $"{result} ";
                history.Text += $"{operation}";
                return;
            }
            double.TryParse(lbDisplay.Text, out double curNum);
            if (!PerformOp(curNum)) return;
            curOperation = ConvertOp(operation);
            displayResult = true;
            lbDisplay.Text = $"{result}";
            history.Text += $" {curNum} {operation}";
            if (history.Text.Count() > 40)
            {
                while (history.Text.Count() >= 40)
                {
                    history.Text = history.Text.Substring(1);
                }
                history.Text = "<< " + history.Text;
            }
        }

        private void btnEquel_Click(object sender, EventArgs e)
        {
            double.TryParse(lbDisplay.Text, out double curNum);
            if (!PerformOp(curNum)) return;
            history.Text = "";
            lbDisplay.Text = $"{result}";
            curOperation = Operation.Sum;
            result = 0;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (exception)
            {
                exception = false;
                SwitchBtns(true);
            }
            curOperation = Operation.Sum;
            result = 0;
            history.Text = "";
            lbDisplay.Text = "0";
        }

        private void btnClearEntry_Click(object sender, EventArgs e)
        {
            if (exception) btnClear_Click(sender, e);
            lbDisplay.Text = "0";
        }

        private bool PerformOp(double number)
        {
            switch (curOperation)
            {
                case Operation.Sum:
                    {
                        result += number;
                    }
                    break;

                case Operation.Subtr:
                    {
                        result -= number;
                    }
                    break;

                case Operation.Mult:
                    {
                        result *= number;
                    }
                    break;
                case Operation.Division:
                    {
                        if (number == 0)
                        {
                            Exception("Devision by 0");
                            history.Text += " 0";
                        }
                        else result /= number;
                    }
                    break;
            }
            if (result >= double.MaxValue) Exception("Error: Too big number");
            return !exception;
        }

        private void Exception(string exMessage)
        {
            lbDisplay.Text = exMessage;
            SystemSounds.Beep.Play();
            exception = true;
            SwitchBtns(false);
        }

        private void SwitchBtns(bool btnsValue)
        {
            btnDiv.Enabled = btnsValue;
            btnMult.Enabled = btnsValue;
            btnSum.Enabled = btnsValue;
            btnPoint.Enabled = btnsValue;
            btnSubtr.Enabled = btnsValue;
            btnEquel.Enabled = btnsValue;
        }



        private Operation ConvertOp(string op)
        {
            switch (op)
            {
                case "+": return Operation.Sum;
                case "-": return Operation.Subtr;
                case "*": return Operation.Mult;
                case "/": return Operation.Division;
                default: return Operation.Default;
            }
        }
    }
}
