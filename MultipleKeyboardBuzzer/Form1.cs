using System.Windows.Forms;
using System.Timers;

namespace MultipleKeyboardBuzzer {
    public partial class Form1 : Form {
        InputDevice id;
        int NumberOfKeyboards;

        bool setTeam1 = false; // boolean to determine when to set a new buzzer button
        bool setTeam2 = false; // boolean to determine when to set a new buzzer button

        string t1b = null; // saves the address for the first buzzer button
        string t2b = null; // saves the address for the second buzzer button

        private static System.Timers.Timer mutex; // mutual exclusion for the two buzzers

        public Form1() {
            InitializeComponent();

            // Create a new InputDevice object, get the number of
            // keyboards, and register the method which will handle the 
            // InputDevice KeyPressed event
            id = new InputDevice(Handle);
            NumberOfKeyboards = id.EnumerateDevices();
            id.KeyPressed += new InputDevice.DeviceEventHandler(m_KeyPressed);

            // Create a timer with a ten second interval.
            mutex = new System.Timers.Timer(5000);

            // Hook up the Elapsed event for the timer.
            mutex.Elapsed += new ElapsedEventHandler(timerTick);
        }

        // The WndProc is overridden to allow InputDevice to intercept
        // messages to the window and thus catch WM_INPUT messages
        protected override void WndProc(ref Message message) {
            if (id != null) {
                id.ProcessMessage(message);
            }
            base.WndProc(ref message);
        }

        private void m_KeyPressed(object sender, InputDevice.KeyControlEventArgs e) {
            string input = e.Keyboard.deviceHandle.ToString() + ":" + e.Keyboard.key.ToString();
            //Replace() is just a cosmetic fix to stop ampersands turning into underlines
            lbHandle.Text = e.Keyboard.deviceHandle.ToString();
            lbType.Text = e.Keyboard.deviceType;
            lbName.Text = e.Keyboard.deviceName.Replace("&", "&&");
            lbDescription.Text = e.Keyboard.Name;
            lbKey.Text = e.Keyboard.key.ToString();
            lbNumKeyboards.Text = NumberOfKeyboards.ToString();
            lbVKey.Text = e.Keyboard.vKey;

            if (input.Equals(t1b)) {
                buzzer1();
            }

            if (input.Equals(t2b)) {
                buzzer2();
            }

            if (setTeam1) {
                t1b = input;
                btnTeam1.Text = "Buzzer 1 festgelegt auf: " + input;
                setTeam1 = false;
            }

            if (setTeam2) {
                t2b = input;
                btnTeam2.Text = "Buzzer 2 festgelegt auf: " + input;
                setTeam2 = false;
            }
        }

        private void btnClose_Click(object sender, System.EventArgs e) {
            this.Close();
        }

        // Set the first buzzer button
        private void btnTeam1_Click(object sender, System.EventArgs e) {
            btnTeam1.Text = "Klicken Sie auf eine beliebige Taste.";
            t1b = null;
            setTeam1 = true;
        }

        // Set the second buzzer button
        private void btnTeam2_Click(object sender, System.EventArgs e) {
            btnTeam2.Text = "Klicken Sie auf eine beliebige Taste.";
            t2b = null;
            setTeam2 = true;
        }

        // reaction to buzzer button 1 pressed
        private void buzzer1() {
            if (!mutex.Enabled) {
                mutex.Start();
                AutoClosingMessageBox.Show("BUZZER 1", "bzzz", 5000);
            }
        }

        //  reaction to buzzer button 2 pressed
        private void buzzer2() {
            if (!mutex.Enabled) {
                mutex.Start();
                AutoClosingMessageBox.Show("BUZZER 2", "bzzz", 5000);
            }
        }

        // timer event to allow buzzer inputs again
        private static void timerTick(object source, ElapsedEventArgs e) {
            mutex.Stop();
        }
    }
}