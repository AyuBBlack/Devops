Насколько я понял задача нужно было сделать 5 колонок у которых будет разного время
создания, следовательно я сделал сначала 5 раз командой:

```
mkdir -p 1/{1..5}
mkdir -p 2/{1..5}
mkdir -p 3/{1..5}
mkdir -p 4/{1..5}
mkdir -p 5/{1..5}
```

После чего нужно было, чтобы время создания файла было одинаковым везде,
следовательно его нужно закинуть во все директории одновременно.
Сделал я это командой: Сделал я это командой:

`touch -mat 202201081215.25 {1..5}/{1..5}/file.txt`

После чего получил следующий результат: После чего получил следующий результат:

```
-rw-r–r-- 1 ls ls 0 Feb 14 22:49 1/1/file.txt
-rw-r–r-- 1 ls ls 0 Feb 14 22:49 1/2/file.txt
-rw-r–r-- 1 ls ls 0 Feb 14 22:49 1/3/file.txt
-rw-r–r-- 1 ls ls 0 Feb 14 22:49 1/4/file.txt
-rw-r–r-- 1 ls ls 0 Feb 14 22:49 1/5/file.txt
```

В заключение нужно было это всё запаковать всё это де В заключение нужно было это всё запаковать всё это дело в архив.

`tar -czvf file.tar.gz *`