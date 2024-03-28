using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Media;
using System.Diagnostics;
using turma.Models.TuringMachine;
using turma.Views;
using MouseButton = Avalonia.Remote.Protocol.Input.MouseButton;

namespace turma.Controls
{
    public class CustomMovableBorder : Border
    {
        public State AssociatedState { get; set; }
        public ToolboxDiagram ToolboxDiagram { get; set; }

        private bool _isPressed;
        private Point _positionInBlock;
        private TranslateTransform _transform = null!;

        public Point centerPos { 
            get { 
                if (_transform == null) { 
                    return this.Bounds.Center; 
                } 
                else { 
                    return this.Bounds.Center.Transform(_transform.Value); 
                } 
            } 
        }

        public CustomMovableBorder(State associatedState, ToolboxDiagram toolboxDiagram)
        {
            AssociatedState = associatedState;
            ToolboxDiagram = toolboxDiagram;
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            if (ToolboxDiagram.MoveBtn.IsChecked.HasValue && ToolboxDiagram.MoveBtn.IsChecked.Value)
            {
                _isPressed = true;
                _positionInBlock = e.GetPosition(Parent);
                if (_transform != null!)
                {
                    _positionInBlock = new Point(
                        _positionInBlock.X - _transform.X,
                        _positionInBlock.Y - _transform.Y);

                }

                base.OnPointerPressed(e);
            }
            
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            _isPressed = false;
            ToolboxDiagram.InvalidateVisual();
            base.OnPointerReleased(e);
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            if (!_isPressed)
                return;

            if (Parent == null)
                return;

            var currentPosition = e.GetPosition(Parent);
            //TODO: circle radius and space for toolbox should be considered in this check 
            if (currentPosition.X > Parent.Bounds.X && currentPosition.Y > Parent.Bounds.Y && currentPosition.X < Parent.Bounds.Size.Width && currentPosition.Y < Parent.Bounds.Size.Height) 
            {
                var offsetX = currentPosition.X - _positionInBlock.X;
                var offsetY = currentPosition.Y - _positionInBlock.Y;
                _transform = new TranslateTransform(offsetX, offsetY);
                RenderTransform = _transform;
                ToolboxDiagram.InvalidateVisual();
            }
            base.OnPointerMoved(e);
        }

        public override string ToString()
        {
            if (AssociatedState != null)
            {
                return AssociatedState.Name;
            }
            else
            {
                return "CustomMovableBorder";
            }

        }
    }
}
