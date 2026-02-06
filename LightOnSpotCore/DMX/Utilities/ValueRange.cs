namespace LightOnSpotCore.DMX.Utilities
{
    public struct ValueRange<T>
    {
        private T left;
        public T Left { get { return left; } set { left = value; } }

        private T right;
        public T Right { get { return right; } set { right = value; } }

        public ValueRange(T left, T right)
        {
            this.left = left;
            this.right = right;
        }
    }
}