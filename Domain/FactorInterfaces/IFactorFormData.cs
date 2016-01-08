using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.FactorInterfaces
{
    //Поддержка анализатором получения дополнительных данных из формы
    public interface IFactorFormData
    {
        string GetUserInstructions();//указания пользователю, которые будут выведены в форме редактирования данных

        //возвращает отфильтрованную копию хранилища, 
        //в которой находятся разрешенные для добавления пользователем данные
        //Т.е. только эти данные выводятся на форму и пользователь может передать их анализатору
        //Например, если анализатор принимает только список лекций - из копии хранилища будут удалены все не-лекции
        //и пользователь на форме редактирования увидит исключетельно лекционные занятия
        EntityStorage FilterStorage(EntityStorage eStorage);
    }
}
