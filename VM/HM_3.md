Часть 1: инфраструктура

1. Создать публичный проект в гитлабе (gitlab.com)
2. Создать шаблонный gitlab-ci.yml в проекте
3. Превратить обе ваши виртуальные машины в Яндекс облаке в ранеры для вашего проекта (если возникают проблемы - пересматриваем лекцию с 65 минуты)
4. Одной из машин присвоить тег: dev
5. Второй из машин присвоить тег: prod

 
Часть 2: проверка
1. Создать 2 джобы
run-on-dev, run-on-prod
которые выводят специфичную для сервера информацию:
- ip, имя, свободное место на машине
- и что-то еще на ваш вкус (например, топ-3 процесса по расходу памяти/cpu)
2. Одну джобу привязать к тегу dev
3. Вторую джобу привязать к тегу prod
4. Запустить пайплайн, и убедиться, что каждая из джобов запустилась на ожидаемом ранере
   
  
Часть 3:
Гордиться собой, вы молодец!

В качестве результата прикрепить пдф-ку:
1. Страница 1:
ссылка на ваш проект в гитлабе,
ссылка на успешно выполненную джобу run-on-dev,
ссылка на успешно выполненную джобу run-on-prod
2. Страница 2: скриншот, подтверждающий что0
run-on-dev действительно запустился на dev runner-е,
run-on-prod действительно запустился на prod runner-е

___

__Первым делом я сделал на своих виртуальных машинах я зарегистрировал gitlab-runner по инструкции на сайте GitLab__

1. 
```
sudo curl -L --output /usr/local/bin/gitlab-runner https://gitlab-runner-downloads.s3.amazonaws.com/latest/binaries/gitlab-runner-linux-amd64
```
2. 
```
sudo chmod +x /usr/local/bin/gitlab-runner
```
3. 
```
sudo useradd --comment 'GitLab Runner' --create-home gitlab-runner --shell /bin/bash
```
4. 
```
sudo gitlab-runner install --user=gitlab-runner --working-directory=/home/gitlab-runner
sudo gitlab-runner start
```

Далее на обоих машинах нужно было либо переименовать, либо удалить файл. 

Я же в свою очередь переименовал.

``mv .bash_logout .bash_logout.backup``

После чего добавил свой токен на обе машины
```
sudo gitlab-runner register --url https://gitlab.com/ --registration-token $REGISTRATION_TOKEN
```
Где вместо $REGISTRATION_TOKEN указал свой токен.

Далее на я стянул свою репозиторий с GitHub на платформу GitLab и добавил в него файл .gitlab-ci.yml для пайплайна.

И реализовал его следующим образом:

```
stages:         
  - first job
  - second job

run-on-dev:       
  stage: first job
  script:
    - echo $(hostname)    
    - echo $(hostname -I)
    - echo $(df -h)
  tags:
    - "dev"
run-on-prod:   
  stage: second job    
  script:
    - hostname    
    - hostname -I
    - df -h
  tags:
    - "prod"

```

Для машины под тегом dev я сделал через echo

Однако, решил для второй джобы проверить работает ли без них, и да, без эко оно тоже работает. Закомитил это всё дело и заработал pipeline.

Ниже лог второй джобы с тегом prod.

```
Running with gitlab-runner 14.10.0 (c6bb62f6)
  on yandex-vm-2 rdQrH-gi
Resolving secrets
00:00
Preparing the "shell" executor
00:00
Using Shell executor...
Preparing environment
00:00
Running on ubu...
Getting source from Git repository
00:01
Fetching changes with git depth set to 20...
Reinitialized existing Git repository in /home/gitlab-runner/builds/rdQrH-gi/0/AyuBBlack/Devops/.git/
Checking out bf7d74ff as main...
Skipping Git submodules setup
Executing "step_script" stage of the job script
00:00
$ hostname
ubu
$ hostname -I
10.0.0.23 172.17.0.1 172.18.0.1 
$ df -h
Filesystem      Size  Used Avail Use% Mounted on
udev            1.9G     0  1.9G   0% /dev
tmpfs           394M  824K  393M   1% /run
/dev/vda2        30G  8.9G   20G  32% /
tmpfs           2.0G     0  2.0G   0% /dev/shm
tmpfs           5.0M     0  5.0M   0% /run/lock
tmpfs           2.0G     0  2.0G   0% /sys/fs/cgroup
tmpfs           394M     0  394M   0% /run/user/1000
tmpfs           394M     0  394M   0% /run/user/1003
Cleaning up project directory and file based variables
00:00
Job succeeded
```
?  
[Ссылка на мой GitLab](https://gitlab.com/AyuBBlack/Devops)