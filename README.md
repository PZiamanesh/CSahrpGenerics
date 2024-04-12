IRepo<T> : IWriteRepo<in T>, IReadRepo<out T>
ListRepo<T> and SqlRepo<T> both inherit IRepo<T>
