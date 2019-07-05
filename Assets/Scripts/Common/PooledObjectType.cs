namespace SD
{
    enum PooledObjectType
    {
        Important,      // this object must be allocated,
                        // if there are no available objects in pool

        NotImportant    // this object can be not allocated,
                        // and another object will be returned
                        // if there are no available objects in pool
    }
}
