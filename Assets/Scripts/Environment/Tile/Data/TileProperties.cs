namespace Game.Environment.Tile.Data
{
    public class TileProperties
    {
        /// <summary>
        /// readonly, stores the raw integer value of the properties
        /// </summary>
        public TileProperty Raw { get; private set; }

        //constructor
        public TileProperties(TileProperty _properties = TileProperty.None)
        {
            Raw = _properties;
        }

        /// <summary>
        /// adds <paramref name="_filter"/> to the stored properties
        /// </summary>
        public void AddProperties(TileProperty _filter)
        {
            //uses an OR operator on all the bits within the enum using the filter
            Raw |= _filter;
        }

        /// <summary>
        /// removes <paramref name="_filter"/> from the stored properties
        /// </summary>
        public void RemoveProperties(TileProperty _filter)
        {
            //inverts the filter and removes it using an AND operator
            Raw &= ~_filter;
        }

        /// <summary>
        /// checks <see cref="properties"/> with <paramref name="_filter"/> to see whether it<br/>
        /// contains the properties within <paramref name="_filter"/>
        /// </summary>
        /// <returns>
        /// <see cref="bool"/> which is <see langword="true"/> whether <see cref="properties"/> matches with <paramref name="_filter"/>
        /// </returns>
        public bool CheckProperties(TileProperty _filter)
        {
            //uses an AND operator on all the bits within the enum using the filter,
            //if it is equal to the integer value in '_filter' then it passes the filter
            return (Raw & _filter) == _filter;
        }
    }
}