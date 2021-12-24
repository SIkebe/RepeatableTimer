using System;
using System.ComponentModel;
using System.Media;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Threading;
using Prism.Commands;
using Prism.Mvvm;
using RepeatableTimer.Enums;

namespace RepeatableTimer.ViewModels
{
    public class MainWindowViewModel : BindableBase, INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            Initialize();

            Timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
            Timer.Tick += Timer_Tick;

            StartCommand = new DelegateCommand(() => { StartTimer(); });
            StopCommand = new DelegateCommand(() => { StopTimer(); });
            PauseCommand = new DelegateCommand(() => { PauseTimer(); });
        }

        public ICommand StartCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand PauseCommand { get; private set; }

        public SoundPlayer Player { get; set; } = new SoundPlayer(Assembly.GetExecutingAssembly().GetManifestResourceStream("RepeatableTimer.Resources.notification4.wav"));
        public DispatcherTimer Timer { get; set; }

        private string _hour;
        public string Hour
        {
            get => _hour;
            set
            {
                if (_hour != value)
                {
                    _hour = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _minute;
        public string Minute
        {
            get => _minute;
            set
            {
                if (_minute != value)
                {
                    _minute = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _secound;
        public string Second
        {
            get => _secound;
            set
            {
                if (_secound != value)
                {
                    _secound = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _round;
        public string Round
        {
            get => _round;
            set
            {
                if (_round != value)
                {
                    _round = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _isStartEnabled;
        public bool IsStartEnabled
        {
            get => _isStartEnabled;
            set
            {
                if (_isStartEnabled != value)
                {
                    _isStartEnabled = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _isStopEnabled;
        public bool IsStopEnabled
        {
            get => _isStopEnabled;
            set
            {
                if (_isStopEnabled != value)
                {
                    _isStopEnabled = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _isPauseEnabled;
        public bool IsPauseEnabled
        {
            get => _isPauseEnabled;
            set
            {
                if (_isPauseEnabled != value)
                {
                    _isPauseEnabled = value;
                    RaisePropertyChanged();
                }
            }
        }

        private CountMode _mode = CountMode.CountUp;
        public CountMode Mode
        {
            get => _mode;
            set => SetProperty(ref _mode, value);
        }

        private bool _isOneTime = true;
        public bool IsOneTime
        {
            get => _isOneTime;
            set => SetProperty(ref _isOneTime, value);
        }

        private int _repeatTimes = 1;
        public int RepeatTimes
        {
            get => _repeatTimes;
            set => SetProperty(ref _repeatTimes, value);
        }

        public int CurrentPeriod { get; set; } = 1;
        public TimeSpan ElapsedTimeSpan { get; set; }
        public TimeSpan OldTimeSpan { get; set; }
        public TimeSpan DesignatedTime { get; private set; }
        public DateTime StartTime { get; private set; }
        public Status Status { get; set; }

        private void StartTimer()
        {
            if (Status != Status.Pause)
            {
                int.TryParse(Hour, out var hour);
                int.TryParse(Minute, out var minute);
                int.TryParse(Second, out var secound);

                DesignatedTime = new TimeSpan(hour, minute, secound);
            }

            Hour = "0";
            Minute = "0";
            Second = "0";

            Timer.Start();
            StartTime = DateTime.Now;
            Round = IsOneTime ?
                "1/1" :
                CurrentPeriod + "/" + RepeatTimes;
            Status = Status.Run;
            IsStartEnabled = false;
            IsPauseEnabled = true;
            IsStopEnabled = true;
        }

        private void PauseTimer()
        {
            OldTimeSpan = OldTimeSpan + ElapsedTimeSpan;
            Timer?.Stop();
            Status = Status.Pause;
            IsStartEnabled = true;
            IsPauseEnabled = false;
            IsStopEnabled = true;
        }

        private void StopTimer()
        {
            OldTimeSpan = new TimeSpan();
            Timer?.Stop();
            Round = string.Empty;
            CurrentPeriod = 1;
            Status = Status.Stop;
            Initialize();
            IsStartEnabled = true;
            IsPauseEnabled = false;
            IsStopEnabled = false;
        }

        bool first = true;
        private void Timer_Tick(object sender, EventArgs e)
        {
            switch (Status)
            {
                case Status.Run:
                    ElapsedTimeSpan = DateTime.Now - StartTime + OldTimeSpan;

                    var ss = Math.Floor((DesignatedTime - ElapsedTimeSpan).TotalSeconds);
                    if (ss == 3 && first)
                    {
                        Player.Play();
                        first = false;
                    }

                    if (DesignatedTime <= ElapsedTimeSpan)
                    {
                        Timer.Stop();
                        OldTimeSpan = new TimeSpan();
                        first = true;

                        CurrentPeriod++;
                        if (!IsOneTime && CurrentPeriod <= RepeatTimes)
                        {
                            Timer.Start();
                            StartTime = DateTime.Now;
                            Round = IsOneTime ?
                                "1/1" :
                                CurrentPeriod + "/" + RepeatTimes;
                            Status = Status.Run;
                            break;
                        }

                        IsStartEnabled = true;
                        IsStopEnabled = false;
                        IsPauseEnabled = false;
                        Status = Status.Stop;
                        CurrentPeriod = 1;
                    }

                    Hour = ElapsedTimeSpan.Hours.ToString();
                    Minute = ElapsedTimeSpan.Minutes.ToString();
                    Second = ElapsedTimeSpan.Seconds.ToString();
                    break;

                case Status.Pause:
                    IsStartEnabled = true;
                    IsStopEnabled = true;
                    IsPauseEnabled = false;
                    break;

                case Status.Stop:
                    Timer.Stop();
                    IsStartEnabled = true;
                    IsStopEnabled = false;
                    IsPauseEnabled = false;
                    break;

                default:
                    break;
            }
        }

        private void Initialize()
        {
            Hour = "0";
            Minute = "0";
            Second = "0";
            Status = Status.Stop;
            IsStartEnabled = true;
            IsStopEnabled = false;
            IsPauseEnabled = false;
        }
    }
}
