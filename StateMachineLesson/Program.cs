using System.Runtime.CompilerServices;

namespace StateMachineLesson;

public class StateMachine
{
    private int state = 0; // Переменная для хранения текущего состояния
    private Task<int> task; // Задача для асинхронной операции
    private TaskAwaiter<int> awaiter; // TaskAwaiter для обработки завершения задачи

    // Метод, управляющий состояниями
    public void MoveNext()
    {
        switch (state)
        {
            case 0:
                // Начало асинхронной операции
                task = SomeLongRunningOperationAsync();
                state = 1; // Обновляем состояние
                awaiter = task.GetAwaiter(); // Получаем TaskAwaiter

                // Если задача еще не завершена, продолжаем выполнение после завершения
                if (!awaiter.IsCompleted)
                {
                    Console.WriteLine("Await complete task...");
                    awaiter.OnCompleted(MoveNext);
                    return; // Ожидаем завершения
                }
                // Если завершена, переходим сразу к case 1
                goto case 1;

            case 1:
                // Получаем результат завершенной задачи
                int result = awaiter.GetResult();
                Console.WriteLine($"Result: {result}");
                // Здесь можно сделать что-то с результатом
                state = -1; // Машина состояний завершена
                break;
        }
    }

    public async Task<int> SomeLongRunningOperationAsync()
    {
        await Task.Delay(15000); // Имитируем долгую операцию
        return 42; // Возвращаем результат
    }
}

class Program
{
    static void Main()
    {
        //var stateMachine = new StateMachine();
        //stateMachine.MoveNext(); // Запускаем state machine

        //// Имитация выполнения в главном потоке
        //Console.WriteLine("Continue in main thread...");

        //// Задержка, чтобы программа не завершилась раньше времени
        //Task.Delay(3000).Wait();
    }
}

public class CustomStateMachine : IAsyncStateMachine
{
    public void MoveNext()
    {
        throw new NotImplementedException();
    }

    public void SetStateMachine(IAsyncStateMachine stateMachine)
    {
        throw new NotImplementedException();
    }
}
