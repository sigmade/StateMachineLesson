using System.Runtime.CompilerServices;

namespace ConsoleApp
{
    internal class SomeClass
    {
        // Исходный код
        //public async Task<int> SomeAsync()
        //{
        //    var number = await InnerAsync();
        //    var customeResult = number * 3;
        //    return customeResult;
        //}

        //public async Task<int> InnerAsync()
        //{
        //    await Task.Delay(2000);
        //    return 5;
        //}

        public Task<int> SomeAsync()
        {
            var stateMachine = new SomeAsyncStateMachine();
            stateMachine.builder = AsyncTaskMethodBuilder<int>.Create();
            stateMachine.state = -1; // Начальное состояние
            stateMachine.outer = this; // Сохранение ссылки на экземпляр класса
            stateMachine.builder.Start(ref stateMachine);
            return stateMachine.builder.Task;
        }

        public Task<int> InnerAsync()
        {
            var stateMachine = new InnerAsyncStateMachine();
            stateMachine.builder = AsyncTaskMethodBuilder<int>.Create();
            stateMachine.state = -1; // Начальное состояние
            stateMachine.outer = this; // Сохранение ссылки на экземпляр класса
            stateMachine.builder.Start(ref stateMachine);
            return stateMachine.builder.Task;
        }

        private struct SomeAsyncStateMachine : IAsyncStateMachine
        {
            public int state;
            public AsyncTaskMethodBuilder<int> builder;
            public SomeClass outer; // Ссылка на экземпляр класса SomeClass
            private TaskAwaiter<int> awaiter;
            private int result;

            public void MoveNext()
            {
                try
                {
                    switch (state)
                    {
                        case -1: // Начальное состояние
                            awaiter = outer.InnerAsync().GetAwaiter();
                            if (!awaiter.IsCompleted)
                            {
                                state = 0; // Следующее состояние
                                builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
                                return;
                            }
                            goto case 0;

                        case 0:
                            result = awaiter.GetResult();
                            var customResult = result * 3;
                            builder.SetResult(customResult); // Устанавливаем результат задачи
                            break;
                    }
                }
                catch (Exception ex)
                {
                    builder.SetException(ex); // Обрабатываем исключение
                }
            }

            public void SetStateMachine(IAsyncStateMachine stateMachine)
            {
                builder.SetStateMachine(stateMachine);
            }
        }

        private struct InnerAsyncStateMachine : IAsyncStateMachine
        {
            public int state;
            public AsyncTaskMethodBuilder<int> builder;
            public SomeClass outer; // Ссылка на экземпляр класса SomeClass
            private TaskAwaiter awaiter;

            public void MoveNext()
            {
                try
                {
                    switch (state)
                    {
                        case -1: // Начальное состояние
                            awaiter = Task.Delay(2000).GetAwaiter();
                            if (!awaiter.IsCompleted)
                            {
                                state = 0; // Следующее состояние
                                builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
                                return;
                            }
                            goto case 0;

                        case 0:
                            awaiter.GetResult();
                            builder.SetResult(5); // Возвращаем результат 5
                            break;
                    }
                }
                catch (Exception ex)
                {
                    builder.SetException(ex); // Обрабатываем исключение
                }
            }

            public void SetStateMachine(IAsyncStateMachine stateMachine)
            {
                builder.SetStateMachine(stateMachine);
            }
        }
    }
}
