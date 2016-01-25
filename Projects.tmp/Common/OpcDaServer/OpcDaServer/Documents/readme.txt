http://iprog.pp.ru/forum/read.php?f=1&i=63611&t=63611

Подскажите где я мог ошибиться!
Создал клиент на базе примера выложенного Федоренко Денисом. Все великолепно читает в синхронном режиме с OPC-сервера Teconic.
Стал проверять на утечку памяти и тут выяснилось, что она имеется и приличная. При комментировании строчки pSyncIO->Read..... утечка исчезает. Полностью этот кусок кода ниже:


  hRes=pItemMgt->QueryInterface(IID_IOPCSYNCIO,(void**)&pSyncIO);

  tagOPCITEMSTATE *pItemValue=NULL;
  hRes=pSyncIO->Read(OPC_DS_CACHE,1,&(ArrayhServer[i]),&pItemValue,&pErrors);
....... обработка данных....
  pSyncIO->Release();
  CoTaskMemFree(pItemValue);

Перед этим для всех тегов делаю AddGroup, tagOPCITEMDEF в AddItems использовал как выделяемую динамически так и статически. Добавлял переменные по одной в AddItems. При этом из возвращаемого параметра pResults сохранял значение hServer в массив ArrayhServer. И далее каждый раз когда надо обновить данные выполняю вышеуказанный кусок кода для всех тегов по очереди.
Что тут не так? Ведь все работает и не вылетает! Но память утекает по 2-3 байта на каждом вызове Read.

Забыл указать, что pErrors после вызова read тоже чистю вызовом CoTaskMemFree(pErrors).

С утечкой памяти разобрался. Все как всегда оказалось проще. Одна из переменных оказалась строковая и после чтения требовалось отдельно делать free с помощью функции SysFreeString.
Вот кусок правильно работающего кода чтения списка параметров из одной группы:
-------------------------------------------------------------------------
hRes=pItemMgt->QueryInterface(IID_IOPCSYNCIO,(void**)&pSyncIO);
if(hRes!=S_OK){
  //обработка ошибки
  return hRes;
}

hRes=pSyncIO->Read(OPC_DS_CACHE,nParam,pListParam->aHServerRes,&aItemValue,&aErrors);
if(hRes!=S_OK){
  //обработка ошибки
}else{
  for(i=0;i<nParam;i++){
    // обработка данных
    if(aItemValue[i].vDataValue.vt==VT_BSTR){
      SysFreeString(aItemValue[i].vDataValue.bstrVal);
    }
  }
}
CoTaskMemFree(aItemValue);
CoTaskMemFree(aErrors);
pSyncIO->Release();
---------------------------------
Естественно такое же освобождение требуется для всех array или блоб и т.д.
