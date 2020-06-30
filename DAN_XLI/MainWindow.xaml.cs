using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;


namespace DAN_XLI
{
    public partial class MainWindow : Window
    {
        #region fields
        private string textInput;
        private string copiesInput;
        //validation logic for number of copies
        public int numberOfCopies
        {
            get
            {
                try
                {
                    int n = Convert.ToInt16(copiesInput);
                    if (n > 0)
                    {
                        return n;
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }

        private BackgroundWorker worker = new BackgroundWorker()
        {
            WorkerReportsProgress = true,
            WorkerSupportsCancellation = true
        };
        #endregion

        #region constructor
        public MainWindow()
        {
            InitializeComponent();
            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += RunWorkerCompleted;
            worker.ProgressChanged += ProgressChanged;
        }
        #endregion

        #region Events of BackgroundWorker class
        /// <summary>
        /// The DoWork event handler is where the time consuming operation runs on the background thread
        /// </summary>
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            int sum = 0;
            string fileName;

                //last step will be larger for this number if 100%numberOfCopies != 0
                int lastStep = 100 % numberOfCopies;

                int step = Convert.ToInt32(100 / numberOfCopies);

                for (int i = 1; i <= numberOfCopies; i++)
                {
                    Thread.Sleep(1000);
                    sum = sum + step;
                    if (i == numberOfCopies)
                    {
                        sum += lastStep;
                    }
                    DateTime dateTime = DateTime.Now;
                    // Calling ReportProgress() method raises ProgressChanged event
                    //passing the percentage of processing that is complete
                    worker.ReportProgress(sum);

                    //create file with textInput content
                    fileName = string.Format(i + "." + dateTime.Day.ToString() + "_" + dateTime.Month.ToString() + "_" + dateTime.Year.ToString() + "_" + dateTime.Hour.ToString() + "_" + dateTime.Minute.ToString() + ".txt");
                    using (StreamWriter sw = File.CreateText(fileName))
                    {
                        sw.WriteLine(textInput);
                    }


                    // Check if the cancellation is requested
                    if (worker.CancellationPending)
                    {
                        // Set Cancel property of DoWorkEventArgs object to true
                        e.Cancel = true;
                        // Reset progress percentage to ZERO and return
                        worker.ReportProgress(0);
                        return;
                    }
                }
            // Store the result in Result property of DoWorkEventArgs object
            e.Result = "Printing process is finished!";
                

        }
        /// <summary>
        /// This event gets raised for 3 reasons: 
        /// 1.When the background worker has completed processing successfully
        /// 2.When the background worker encountered an error
        /// 3.When the background worker is requested to cancel the execution
        /// </summary>
        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //3.
            if (e.Cancelled)
            {
                label2.Content = "Printing cancelled";
                btnCancel.IsEnabled = false;
            }
            //2.
            else if (e.Error != null)
            {
                label1.Content = e.Error.Message;
            }
            //1.
            else
            {
                label1.Content = e.Result.ToString();
                btnCancel.IsEnabled = false;
                label2.Content = null;
            }

        }

        /// <summary>
        /// The ProgressChanged event handler is where we write code to update the user interface elements with the progress made so far.
        /// </summary>
        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            label1.Content = e.ProgressPercentage.ToString() + "%";
        }
        #endregion


        #region Events handles for button clicks and text inputs
        //get text input 
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Enable or disable Print button depending on the textInput and numberOfCopies
            textInput = text.Text;
            if (!String.IsNullOrEmpty(textInput) && numberOfCopies != 0)
            {
                btnPrint.IsEnabled = true;
            }
            else
            {
                btnPrint.IsEnabled = false;
            }

        }

        //get copies input
        private void CurrentCopies_TextChanged(object sender, TextChangedEventArgs e)
        {
            copiesInput = copies.Text;
            if (!String.IsNullOrEmpty(textInput) && numberOfCopies != 0)
            {
                btnPrint.IsEnabled = true;
            }
            else
            {
                btnPrint.IsEnabled = false;
            }

        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            // Check if the backgroundWorker is already busy running the asynchronous operation
            if (!worker.IsBusy)
            {
                // This method will start the execution asynchronously in the background
                worker.RunWorkerAsync();
                btnCancel.IsEnabled = true;
            }
            else
            {
                label2.Content = "Printing in progress...";
            }


        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (worker.IsBusy)
            {
                // Cancel the asynchronous operation if still in progress
                worker.CancelAsync();
            }

        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
        #endregion

    }
}
