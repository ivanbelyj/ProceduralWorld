using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IDispatcher
{
    /// <summary>
    /// Планирует код для выполнения в главном потоке
    /// </summary>
    void Enqueue(Action action);
    
    Task<T> Enqueue<T>(Func<T> func);
}
