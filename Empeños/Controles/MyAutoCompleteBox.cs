using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;

namespace Empeños.Controles
{
    public class MyAutoCompleteBox : AutoCompleteBox
    {
        public void SelectAll()
        {
            var textbox = Template.FindName("Text", this) as TextBox;
            textbox.SelectAll();
            if (textbox != null) textbox.Focus();
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);

            if (!IsDropDownOpen)
                SelectAll();
        }
    }

    public class NumericTextBox : TextBox
    {
        public NumericTextBox()
        {
            TextAlignment = System.Windows.TextAlignment.Right;
            minValue = double.NaN;
            maxValue = double.NaN;
            valueType = ValueTypes.Integer;
            mask = "0:###,##0";
        }

        #region MinimumValue Property

        private double minValue;

        public double MinValue
        {
            get { return minValue; }
            set
            {
                minValue = value;

                if (!value.Equals(double.NaN))
                {
                    switch (ValueType)
                    {
                        case ValueTypes.Integer:
                            if (value < Convert.ToDouble(Int32.MinValue))
                                throw new ArgumentOutOfRangeException("Overflow, minimum: " + Int32.MinValue.ToString());
                            break;

                        case ValueTypes.Double:
                            if (value < (Double.MinValue / 100))
                                throw new ArgumentOutOfRangeException("Overflow, minimum: " + (Double.MinValue / 100).ToString());
                            break;
                    }
                }

                ValidateTextBox();
            }
        }

        #endregion

        #region MaximumValue Property

        private double maxValue;

        public double MaxValue
        {
            get { return maxValue; }
            set
            {
                maxValue = value;

                if (!maxValue.Equals(double.NaN))
                {
                    switch (ValueType)
                    {
                        case ValueTypes.Integer:
                            if (maxValue > Convert.ToDouble(Int32.MaxValue))
                                throw new ArgumentOutOfRangeException("Overflow, maximum: " + Int32.MaxValue.ToString());
                            break;

                        case ValueTypes.Double:
                            //We stop two characters ahead, so as not to cause an exception.
                            if (maxValue > (Double.MinValue / 100))
                                throw new ArgumentOutOfRangeException("Overflow, maximum: " + (Double.MaxValue / 100).ToString());
                            break;
                    }
                }

                ValidateTextBox();
            }
        }

        #endregion

        #region Mask Property

        private string mask;

        public string Mask
        {
            get { return mask; }
            set
            {
                mask = value;

                ValidateTextBox();
            }
        }

        #endregion

        #region ValueType Property

        private ValueTypes valueType;

        [DefaultValue(ValueTypes.Integer)]
        public ValueTypes ValueType
        {
            get { return valueType; }
            set
            {
                valueType = value;
                mask = valueType == ValueTypes.Integer ? "0:0" : "0:#,0";

                ValidateTextBox();
            }
        }

        #endregion

        [EditorBrowsable(EditorBrowsableState.Never)]
        public int AsInt
        {
            get
            {
                if (int.TryParse(Text.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, String.Empty), out int resultado))
                    return resultado;
                else
                    return 0;
            }
            set
            {
                Text = value.ToString();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public decimal AsDecimal
        {
            get
            {
                if (decimal.TryParse(Text.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, String.Empty), out decimal resultado))
                    return resultado;
                else
                    return 0;
            }
            set
            {
                Text = value.ToString();
            }
        }

        private void ValidateTextBox()
        {
            if (string.Empty != Mask)
                Text = ValidateValue(Mask, ValueType, Text, MinValue, MaxValue);
        }

        protected override void OnGotKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            if (Text.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator))
                CaretIndex = Text.IndexOf(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
            else
                CaretIndex = Text.Length;

            base.OnGotKeyboardFocus(e);
        }

        protected override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if ((ValueTypes.Text != ValueType) && (Key.Space == e.Key))
            {
                //Space is not allowed at number type entry.
                e.Handled = true;
                return;
            }

            if (Key.Back == e.Key)
            {
                //Backspace
                if ((0 == SelectionLength) &&
                    (0 < CaretIndex))
                {
                    //If nothing is selected, the cursor is not at the very beginning.
                    if (NumberFormatInfo.CurrentInfo.NumberDecimalSeparator == Text.Substring(CaretIndex - 1, 1))
                    {
                        //This does not have to be carried out if we want to delete the separator
                        CaretIndex -= 1;
                        e.Handled = true;
                        return;
                    }

                    if ((true == Text.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)) &&
                        (CaretIndex > Text.IndexOf(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) + 1))
                    {
                        //If the cursor is at the decimal value and we delete backwards.
                        int caret = CaretIndex;
                        Text = Text.Substring(0, CaretIndex - 1) + Text.Substring(CaretIndex) + "0";
                        CaretIndex = caret - 1;
                        e.Handled = true;
                        return;
                    }
                }

                if (0 < CaretIndex)
                {
                    if (0 < SelectionLength)
                    {
                        //If we delete the highlighted part of text.
                        int caret = SelectionStart;
                        int rcaret = Text.Length - caret - SelectionLength;

                        string txtWS = Text.Substring(0, caret);
                        string txtWOS = txtWS.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, String.Empty);

                        string txtSWS = Text.Substring(caret, SelectionLength);
                        string txtSWOS = txtSWS.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, String.Empty);

                        string text = Text.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, String.Empty);

                        text = text.Substring(0, caret - (txtWS.Length - txtWOS.Length)) +
                               //If the highlighted part contains the decimal separator, we put it back after deleting.
                               (txtSWOS.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) ? NumberFormatInfo.CurrentInfo.NumberDecimalSeparator : String.Empty) +
                               text.Substring(caret - (txtWS.Length - txtWOS.Length) + SelectionLength - (txtSWS.Length - txtSWOS.Length));

                        Text = text;

                        if (txtSWOS.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator))
                        {
                            //If the decimal separator was also selected, then the cursor is put in front of the decimal separator.
                            caret = Text.IndexOf(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
                        }
                        else
                        {
                            caret = Text.Length - rcaret;
                        }

                        if (caret < 0)
                            caret = 0;

                        CaretIndex = caret;
                        e.Handled = true;
                        return;
                    }
                    else
                    {
                        //One item is deleted from the left.
                        int caret = CaretIndex;
                        int rcaret = Text.Length - caret;

                        string txtWS = Text.Substring(0, caret);
                        string txtWOS = txtWS.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, String.Empty);

                        string text = Text.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, String.Empty);

                        text = text.Substring(0, caret - (txtWS.Length - txtWOS.Length) - 1) +
                               text.Substring(caret - (txtWS.Length - txtWOS.Length));

                        Text = text;

                        caret = Text.Length - rcaret;

                        if (caret < 0)
                            caret = 0;

                        CaretIndex = caret;
                        e.Handled = true;
                        return;

                    }
                }

            }

            if (Key.Delete == e.Key)
            {
                //Del               
                if ((0 == SelectionLength) && (CaretIndex < Text.Length))
                {
                    //If nothing is selected, the cursor is not at the very end.
                    if (NumberFormatInfo.CurrentInfo.NumberDecimalSeparator == Text.Substring(CaretIndex, 1))
                    {
                        //This does not have to be carried out if we want to delete the separator
                        CaretIndex += 1;
                        e.Handled = true;
                        return;
                    }

                    if ((true == Text.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)) &&
                        (CaretIndex > Text.IndexOf(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)))
                    {
                        //If the cursor is at the decimal value and we delete.
                        int caret = CaretIndex;
                        int ind = Text.IndexOf(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);

                        Text = Text.Substring(0, caret) + Text.Substring(caret + 1) + "0";
                        CaretIndex = caret;
                        e.Handled = true;
                        return;
                    }
                }

                if (0 < SelectionLength)
                {
                    //If we delete the highlighted part of text.
                    int caret = SelectionStart;
                    int rcaret = Text.Length - caret - SelectionLength;

                    string txtWS = Text.Substring(0, caret);
                    string txtWOS = txtWS.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, String.Empty);

                    string txtSWS = Text.Substring(caret, SelectionLength);
                    string txtSWOS = txtSWS.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, String.Empty);

                    string text = Text.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, String.Empty);

                    text = text.Substring(0, caret - (txtWS.Length - txtWOS.Length)) +
                           //If the highlighted part contains the decimal separator, we put it back after deleting.
                           (txtSWOS.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) ? NumberFormatInfo.CurrentInfo.NumberDecimalSeparator : String.Empty) +
                           text.Substring(caret - (txtWS.Length - txtWOS.Length) + SelectionLength - (txtSWS.Length - txtSWOS.Length));

                    //If there is only one decimal separator, it will be deleted.
                    text = (NumberFormatInfo.CurrentInfo.NumberDecimalSeparator == text ? String.Empty : text);

                    Text = text;

                    if (txtSWOS.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator))
                    {
                        caret = Text.IndexOf(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
                    }
                    else
                    {
                        caret = Text.Length - rcaret;
                    }

                    if (caret < 0)
                        caret = 0;

                    CaretIndex = caret;
                    e.Handled = true;
                    return;
                }
                else
                {
                    if (CaretIndex < Text.Length)
                    {

                        //One item is deleted from the right.
                        int caret = CaretIndex;
                        int rcaret = Text.Length - caret - 1;

                        if (NumberFormatInfo.CurrentInfo.NumberGroupSeparator == Text.Substring(caret, 1))
                            rcaret -= 1;

                        string txtWS = Text.Substring(0, caret);
                        string txtWOS = txtWS.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, String.Empty);

                        string text = Text.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, String.Empty);

                        text = text.Substring(0, caret - (txtWS.Length - txtWOS.Length)) +
                               text.Substring(caret - (txtWS.Length - txtWOS.Length) + 1);

                        Text = text;
                        caret = Text.Length - rcaret;

                        if (caret < 0)
                            caret = 0;

                        CaretIndex = caret;
                        e.Handled = true;
                        return;
                    }

                }


            }

            e.Handled = false;
        }

        protected override void OnPreviewTextInput(System.Windows.Input.TextCompositionEventArgs e)
        {
            bool isValid = IsSymbolValid(Mask, e.Text, ValueType);
            bool textInserted = false;
            bool toNDS = false;

            if (isValid)
            {
                //Current content
                string txtOld = Text.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, String.Empty);
                //New content
                string txtNew = String.Empty;
                bool handled = false;
                int caret = CaretIndex;
                int rcaret = 0;

                if (e.Text == NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                {
                    //If we entered a decimal separator.
                    int ind = Text.IndexOf(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) + 1;
                    rcaret = Text.Length - ind;
                    //The text doesn't change.
                    txtNew = txtOld;
                    handled = true;
                }

                if ((!handled) && (e.Text == NumberFormatInfo.CurrentInfo.NegativeSign))
                {
                    //We entered a negative symbol.
                    if (Text.Contains(NumberFormatInfo.CurrentInfo.NegativeSign))
                    {
                        //A negative symbol is already in the text.
                        //As overriding the text initializes the cursor, the present position is remembered.
                        rcaret = Text.Length - caret;
                        txtNew = txtOld.Replace(NumberFormatInfo.CurrentInfo.NegativeSign, string.Empty);
                    }
                    else
                    {
                        //There is no negative symbol in the text.
                        //As overriding the text initializes the cursor, the present position is remembered.
                        rcaret = Text.Length - caret;
                        txtNew = NumberFormatInfo.CurrentInfo.NegativeSign + txtOld;
                    }
                    handled = true;
                }

                if (!handled)
                {
                    textInserted = true;
                    if (0 < SelectionLength)
                    {
                        //We delete the highlighted text and insert what we have just written.
                        int ind = SelectionStart;
                        rcaret = Text.Length - ind - SelectionLength;

                        string txtWS = Text.Substring(0, ind);
                        string txtWOS = txtWS.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, String.Empty);

                        string txtSWS = Text.Substring(ind, SelectionLength);
                        string txtSWOS = txtSWS.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, String.Empty);

                        string txtNWS = e.Text;
                        string txtNWOS = txtNWS.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, String.Empty);

                        txtNew = txtOld.Substring(0, ind - (txtWS.Length - txtWOS.Length)) + txtNWOS +
                                (txtSWOS.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) ? NumberFormatInfo.CurrentInfo.NumberDecimalSeparator : String.Empty) +
                                txtOld.Substring(ind - (txtWS.Length - txtWOS.Length) + SelectionLength - (txtSWS.Length - txtSWOS.Length));

                        if (txtSWOS.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator))
                        {
                            //If the decimal separator was also highlighted, then the cursor is put in front of the decimal separator.
                            toNDS = true;
                        }

                    }
                    else
                    {
                        //We insert the character to the right of the cursor.
                        int ind = CaretIndex;
                        rcaret = Text.Length - ind;

                        if ((0 < rcaret) &&
                            (NumberFormatInfo.CurrentInfo.NumberGroupSeparator == Text.Substring(ind, 1)))
                            rcaret -= 1;

                        string txtWS = Text.Substring(0, ind);
                        string txtWOS = txtWS.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, String.Empty);

                        string txtNWS = e.Text;
                        string txtNWOS = txtNWS.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, String.Empty);

                        txtNew = txtOld.Substring(0, ind - (txtWS.Length - txtWOS.Length)) + txtNWOS +
                                txtOld.Substring(ind - (txtWS.Length - txtWOS.Length));
                    }
                }

                try
                {
                    double val = Double.Parse(txtNew);
                    double newVal = ValidateLimits(MinValue, MaxValue, val, ValueType);
                    if (val != newVal)
                    {
                        txtNew = newVal.ToString();
                    }
                    else if (val == 0)
                    {
                        if (!txtNew.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator))
                            txtNew = "0";
                    }
                }
                catch
                {
                    txtNew = "0";
                }

                Text = txtNew;

                if ((true == Text.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)) &&
                    (caret > Text.IndexOf(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)))
                {
                    //If the cursor is at the decimal value, then it moves to the right of the decimal separator, if possible.
                    if (caret < Text.Length)
                    {
                        if (textInserted)
                        {
                            caret += 1;
                            rcaret = Text.Length - caret;
                        }
                    }
                    else
                    {
                        //We are at the very end; it's not possible to enter more characters.
                        if (textInserted)
                            Text = txtOld;
                    }
                }

                caret = Text.Length - rcaret;

                if (caret < 0)
                    caret = 0;

                if (toNDS)
                {
                    CaretIndex = Text.IndexOf(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
                }
                else
                {
                    CaretIndex = caret;
                }

            }

            e.Handled = true;
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            string text = Text.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, String.Empty);

            string mask = Mask;
            ValueTypes vt = ValueType;

            if (0 != mask.Length)
            {
                if (0 < Text.Length)
                {
                    if (vt.Equals(ValueTypes.Integer))
                    {
                        Text = String.Format("{" + mask + "}", Int32.Parse(text));
                        e.Handled = true;
                    }
                    else
                    {
                        Text = String.Format("{" + mask + "}", Double.Parse(text));
                        e.Handled = true;
                    }
                }
                else
                {
                    Text = "";
                    e.Handled = true;
                }
            }
        }

        private string ValidateValue(string mask, ValueTypes vt, string value, double min, double max)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            value = value.Trim();

            switch (vt)
            {
                case ValueTypes.Integer:
                    try
                    {
                        value = ValidateLimits(min, max, Int32.Parse(value), vt).ToString();
                        return value;
                    }
                    catch { }
                    return string.Empty;

                case ValueTypes.Double:
                    try
                    {
                        value = ValidateLimits(min, max, Double.Parse(value), vt).ToString();
                        return value;
                    }
                    catch { }
                    return string.Empty;
            }
            return string.Empty;

        }

        private double ValidateLimits(double min, double max, double value, ValueTypes vt)
        {
            if (!min.Equals(double.NaN))
            {
                if (value < min)
                    return min;
            }
            else
            {
                switch (vt)
                {
                    case ValueTypes.Integer:
                        if (value < Convert.ToDouble(Int32.MinValue))
                            return Convert.ToDouble(Int32.MinValue);
                        break;

                    case ValueTypes.Double:
                        //Két karakterrel előbb megállunk, hogy ne okozzon exception-t.
                        if (value < (Double.MinValue / 100))
                            return (Double.MinValue / 100);
                        break;
                }
            }

            if (!max.Equals(double.NaN))
            {
                if (value > max)
                    return max;
            }
            else
            {
                switch (vt)
                {
                    case ValueTypes.Integer:
                        if (value > Convert.ToDouble(Int32.MaxValue))
                            return Convert.ToDouble(Int32.MaxValue);
                        break;

                    case ValueTypes.Double:
                        //We stop two characters ahead, so as not to cause an exception.
                        if (value > (Double.MaxValue / 100))
                            return (Double.MaxValue / 100);
                        break;
                }
            }
            return value;
        }

        private bool IsSymbolValid(string mask, string str, ValueTypes typ)
        {
            switch (typ)
            {
                case ValueTypes.Text:
                    return true;
                case ValueTypes.Integer:
                    if (str == NumberFormatInfo.CurrentInfo.NegativeSign)
                        return true;
                    break;
                case ValueTypes.Double:
                    if (str == NumberFormatInfo.CurrentInfo.NumberDecimalSeparator ||
                        str == NumberFormatInfo.CurrentInfo.NegativeSign)
                        return true;
                    break;
            }

            if (typ.Equals(ValueTypes.Integer) || typ.Equals(ValueTypes.Double))
            {
                foreach (char ch in str)
                {
                    if (!Char.IsDigit(ch))
                        return false;
                }

                return true;
            }

            return false;

        }
    }

    public enum ValueTypes
    {
        Text,
        Integer,
        Double
    }
}
