
IWriteRepo<in T>       IReadRepo<out T>
            ^          ^
            |          |
              IRepo<T>
              ^      ^
              |      |
    ListRepo<T>      SqlRepo<T>          
