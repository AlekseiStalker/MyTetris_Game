using System;  

namespace TetrisLibrary {

    public delegate void StateHandler();

    public class GameField : Board {

        private FigureManager _manager; 
        private Figure _curFigure;
        private Figure _nextFigure;

        private bool _isFalling;

        /// <summary>
		/// ������� ����� ��������� ������
		/// </summary> 
        public event StateHandler OnStateChanged;
        /// <summary>
        /// ������� ����������(������������) ����� ������ �� ������� ����
        /// </summary> 
        public event StateHandler OnAddFigure;

        public GameField(int heigth, int width) : base(heigth, width) {

            _manager = new FigureManager();

            _nextFigure = GetRandomFigure();
        }

        /// <summary>
		/// ���������� ��������� ������
		/// </summary> 
        public Figure GetRandomFigure() {
            return _manager.GetRandom();
        }

        /// <summary>
        /// ���������� ��������� �� ������� ������ 
        /// </summary> 
        public Figure GetNextFigure() {
            return _nextFigure.Clone();
        }

        /// <summary>
		/// �������� ����� ������ ������, ���������� ����
		/// </summary> 
		/// <returns>true, ���� ������ ������� ��������� �� ����, ����� - false</returns>
        public bool PalaceFigure() {
            _curFigure = _nextFigure.Clone();
            _nextFigure = GetRandomFigure();

            _shiftCol = BoardWidth / 2 - 1;
            _shiftRow = 0;

            for (int row = 0; row < _curFigure.FigureHeigth; row++) {
                for (int col = 0; col < _curFigure.FigureWidth; col++) {
                    if (_curFigure[row, col] != 0) {
                        if (board[row, col + _shiftCol] == CellColor.Default) {
                            board[row, col + _shiftCol] = _curFigure.Color;
                        }
                        else { 
                            return false;
                        } 
                    }
                }
            } 

            if (OnStateChanged != null) {  
                OnStateChanged();
            }
             
            return true;
        }

        /// <summary>
		/// ������� ���� ����, ���� �� ���������� �����������
		/// </summary> 
        public void Drop() {
            if (_curFigure != null) {
                while (_isFalling) {
                    MoveDown();
                } 
            }

            _isFalling = true;
            if (OnStateChanged != null) {
                OnStateChanged();
            }
        }

        /// <summary>
		/// ������� ������ ���� �� ���� ������
		/// </summary> 
        public void DoStep() {
            if (_curFigure != null) {
                MoveDown();
            }  
        }

        protected bool CanMoveDown() {
            _shiftRow++;

            for (int row = 0; row < _curFigure.FigureHeigth; row++) {
                for (int col = 0; col < _curFigure.FigureWidth; col++) {
                    if (_curFigure[row, col] != 0) {
                        if (!IsCellEqual(row + _shiftRow, col + _shiftCol, CellColor.Default)) { 
                            _shiftRow--;
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        protected bool CanMoveRight() {
            _shiftCol++;

            for (int row = 0; row < _curFigure.FigureHeigth; row++) {
                for (int col = 0; col < _curFigure.FigureWidth; col++) {
                    if (_curFigure[row, col] != 0) {
                        if (!IsCellEqual(row + _shiftRow, col + _shiftCol, CellColor.Default)) {
                            _shiftCol--;
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        protected bool CanMoveLeft() {
            _shiftCol--;

            for (int row = 0; row < _curFigure.FigureHeigth; row++) {
                for (int col = 0; col < _curFigure.FigureWidth; col++) {
                    if (_curFigure[row, col] != 0) {
                        if (!IsCellEqual(row + _shiftRow, col + _shiftCol, CellColor.Default)) { 
                            _shiftCol++;
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        protected bool CanRotate() {
            Figure rotatedArr = _curFigure.Clone();
            rotatedArr.RotateFigure();

            for (int row = 0; row < _curFigure.FigureHeigth; row++) {
                for (int col = 0; col < _curFigure.FigureWidth; col++) {
                    if (rotatedArr[row, col] != 0) {
                        if (!IsCellEqual(row + _shiftRow, col + _shiftCol, CellColor.Default)) {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// ������������ ������� ������, ���� ��� ��������
        /// </summary> 
        public void Rotate() {
            ClearCurrentFigure();

            if (CanRotate()) {
                _curFigure.RotateFigure();
                for (int row = 0; row < _curFigure.FigureHeigth; row++) {
                    for (int col = 0; col < _curFigure.FigureWidth; col++) {
                        if (_curFigure[row, col] != 0) {
                            board[row + _shiftRow, col + _shiftCol] = _curFigure.Color;
                        }
                    }
                }
                if (OnStateChanged != null) {
                    OnStateChanged();
                }
            } else {
                RevertCurrentFigure();
            }
        }
          
        /// <summary>
        /// ������� ������� ������ ������ �� ���� ������, ���� ��� ��������
        /// </summary> 
        public void MoveRigth() {
            ClearCurrentFigure();

            if (CanMoveRight()) { 
                for (int row = 0; row < _curFigure.FigureHeigth; row++) {
                    for (int col = 0; col < _curFigure.FigureWidth; col++) {
                        if (_curFigure[row, col] != 0) { 
                            board[row + _shiftRow, col + _shiftCol] = _curFigure.Color; 
                        }
                    }
                }
                if (OnStateChanged != null) {
                    OnStateChanged();
                }
            } else {
                RevertCurrentFigure();
            }
        }

        /// <summary>
        /// ������� ������� ������ ����� �� ���� ������, ���� ��� ��������.
        /// </summary> 
        public void MoveLeft() {
            ClearCurrentFigure();

            if (CanMoveLeft()) {
                for (int row = 0; row < _curFigure.FigureHeigth; row++) {
                    for (int col = 0; col < _curFigure.FigureWidth; col++) {
                        if (_curFigure[row, col] != 0) {
                            board[row + _shiftRow, col + _shiftCol] = _curFigure.Color;
                        }
                    }
                }
                if (OnStateChanged != null) {
                    OnStateChanged();
                }
            } else {
                RevertCurrentFigure();
            }
        }

        /// <summary>
        /// ������� ������� ���� �� ���� ������, ���� ��� ��������. 
        /// � ��������� ������ �������� ������� �� �������� ����� ������ �� ���� � ������������� �������.
        /// </summary> 
        public void MoveDown() {
            ClearCurrentFigure();

            if (CanMoveDown()) {
                for (int row = 0; row < _curFigure.FigureHeigth; row++) {
                    for (int col = 0; col < _curFigure.FigureWidth; col++) {
                        if (_curFigure[row, col] != 0) {
                            board[row + _shiftRow, col + _shiftCol] = _curFigure.Color;
                        }
                    }
                }
                if (OnStateChanged != null) {
                    OnStateChanged();
                }
            } else {
                RevertCurrentFigure();

                _isFalling = false;
                OnAddFigure();
            }
        }

        private void ClearCurrentFigure() { 
            for (int row = 0; row < _curFigure.FigureHeigth; row++) {
                for (int col = 0; col < _curFigure.FigureWidth; col++) {
                    if (_curFigure[row, col] != 0) {
                        board[row + _shiftRow, col + _shiftCol] = CellColor.Default;
                    }
                }
            } 
        }

        private void RevertCurrentFigure() { 
            for (int row = 0; row < _curFigure.FigureHeigth; row++) {
                for (int col = 0; col < _curFigure.FigureWidth; col++) {
                    if (_curFigure[row, col] != 0) {
                        board[row + _shiftRow, col + _shiftCol] = _curFigure.Color; 
                    }
                }
            } 
        }

        public override void Clear() {
            base.Clear();
            if (OnStateChanged != null) {
                OnStateChanged();
            } 
            _curFigure = null;
        }
    }
}