using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace WPFDemo {
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window {
    public MainWindow() {
      InitializeComponent();

      var movePoints =
        from ev in Observable.FromEventPattern<MouseEventArgs>(this, "MouseMove")
        select ev.EventArgs.GetPosition(this);

            // Step 1 - this shows the mouse position in the label
            //movePoints.ObserveOnDispatcher( ).Subscribe(
            //  pos => label.Content = pos.ToString( ));

            var mouseDownPoints =
              from ev in Observable.FromEventPattern<MouseEventArgs>(this, "MouseLeftButtonDown")
              select ev.EventArgs.GetPosition(this);

            var mouseUpPoints =
              from ev in Observable.FromEventPattern<MouseEventArgs>(this, "MouseLeftButtonUp")
              select ev.EventArgs.GetPosition(this);

            // Step 2 - reacts to the mouse down event and shows position until the 
            // mouse button is released - unfortunately, this technique works only
            // one time

            //var query =
            //  from pos in movePoints.SkipUntil(mouseDownPoints).TakeUntil(mouseUpPoints)
            //  select pos;
            //query.ObserveOnDispatcher( ).Subscribe(
            //  pos => label.Content = pos.ToString( ));

            // Step 3 - using a more complicated query, the mechanism works more than once
            //var query =
            //  from startPos in mouseDownPoints
            //  from pos in movePoints.StartWith(startPos).TakeUntil(mouseUpPoints)
            //  select pos;

            // Alternatively:
            //var query_ =
            //  mouseDownPoints.SelectMany(startPos =>
            //    movePoints.StartWith(startPos).TakeUntil(mouseUpPoints));

            //query.ObserveOnDispatcher( ).Subscribe(
            //  pos => label.Content = pos.ToString( ));

            // Step 4 - a moveable UI label

            label.Content = "move me";

            var mouseDown = mouseDownPoints.Where(sp =>
              sp.X > label.Margin.Left &&
              sp.X < label.Margin.Left + label.ActualWidth &&
              sp.Y > label.Margin.Top &&
              sp.Y < label.Margin.Top + label.ActualHeight);

            var mouseMove = movePoints.Zip(movePoints.Skip(1), (prev, cur) => new { dx = cur.X - prev.X, dy = cur.Y - prev.Y });

            var deltas = from md in mouseDown
                         from mm in mouseMove.TakeUntil(mouseUpPoints)
                         select mm;

            deltas.ObserveOnDispatcher().Subscribe(
              delta =>
              {
                  label.Margin = new Thickness(label.Margin.Left + delta.dx, label.Margin.Top + delta.dy, 0, 0);
              });
        }

    private void button1_Click(object sender, RoutedEventArgs e) {
      var ob = Observable.Interval(TimeSpan.FromSeconds(0.1));

      // Step 1 - this doesn't work because we'll be updating the UI from the 
      // wrong thread

      //ob.Subscribe(
      //  v => output.Text += Environment.NewLine + String.Format(
      //    "(Thread {0}) {1}", Thread.CurrentThread.ManagedThreadId, v));

      // Step 2 - this works by passing in a particular sync context

      //ob.ObserveOn(SynchronizationContext.Current).Subscribe(
      //  v => output.Text += Environment.NewLine + String.Format(
      //    "(Thread {0}) {1}", Thread.CurrentThread.ManagedThreadId, v));

      // Step 3 - finds the correct sync context automatically

      //ob.ObserveOnDispatcher().Subscribe(
      //  v => output.Text += Environment.NewLine + String.Format(
      //    "(Thread {0}) {1}", Thread.CurrentThread.ManagedThreadId, v));
    }
  }
}
