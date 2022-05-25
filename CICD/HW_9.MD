Что нужно сделать:
0. Сформулируйте, что для вас значит "корректно развернутое приложение"
1. Написать свой инвентори и плейбук, глобальная цель которого - развернуть ваше .net приложение на ваших машинах в клауде (на каждую машину по инстансу приложения)
2. Развертка приложения должна быть через докер
Тут есть 2 варианта:
1) Либо вы будете вынуждены почитать про модуль докера в ансибле и вынужденно им воспользоваться.
2) Либо вы сделаете костыль на командах.
Первый путь больше в парадигме ансибла + даст больше опыта изучения модулей ансибла.
Второй путь менее болезненный (на мой взгляд).
Засчитана будет любая реализация. явно выраженного акцента на этом делать не вижу смысла.
3. Добавить плейбук в ваш репозиторий
4. Выполните плейбук со своего хоста.
Логи сохраните в файл или скриншот. (можно выполнить команду с перенаправлением потока логов в файл, например)
5. Сделайте скрины подтверждающие успешность запуска
(Я предлагаю такой набор: список поднятых контейнеров + доступность приложения на каждом из хостов)

---

### **Для начала я установил ansible на свой раннер.**

`sudo apt install ansible`

> После чего для тестирования создал директорию **ansible** 
> 
> И создал вот такой **hosts файл** *(в дальнейшем он будет пустой и будет заполняться в ходе одного из этапов в пайплайне)*: 

```
[stage]
vm-1 ansible_host=51.250.79.127

[prod]
cloud-1 ansible_host=178.154.204.217

[yan]
vm-2 ansible_host=51.250.72.33

[main:children]
stage
prod
yan
```

> А также директорию **group_vars**, в котором создал файд **main** где для всех общие переменные, в частности ключ для удобства тестирования оставил для всех одинаковый.

```
ansible_user         : ubuntu
ansible_ssh_key_file : /home/ubuntu/.ssh/id_rsa
environment          : Main
owner                : Ayub
```

> Далее создаю директорию **roles** в котором выполяняю команду: 

`ansible-galaxy init deploy-app`

> В итоге у меня получается вот такая вот роль, теперь можно приступать к основным командам для деплоя моего 
> приложения.

![](https://i.imgur.com/d6V3Uuf.png)

> Для данной роли я создаю такс в котором я устанавливаю докер и докер композ, а также копирую все файлы для разравочивания всех сервисов в докер композ, которые у меня были в [домашке 7.](https://gitlab.com/AyuBBlack/inno/-/blob/main/CICD/HW_7.MD) 

```
#Устанавливаю зависимости докера
- name: Install dependencies
  apt:
    name: "{{item}}"
    state: present
    update_cache: yes
  loop:
    - apt-transport-https
    - ca-certificates
    - curl
    - gnupg-agent
    - software-properties-common

- name: Add GPG key
  apt_key:
    url: https://download.docker.com/linux/ubuntu/gpg
    state: present

- name: Add repository docker
  apt_repository:
    repo: deb https://download.docker.com/linux/ubuntu bionic stable
    state: present

- name: Install docker
  apt:
    name: "{{item}}"
    state: latest
    update_cache: yes
  loop:
    - docker-ce
    - docker-ce-cli
    - containerd.io

- name: Install docker-compose from official github repo
  get_url:
    url : https://github.com/docker/compose/releases/download/1.29.2/docker-compose-Linux-x86_64
    dest: /usr/local/bin/docker-compose
    mode: 7777

- name: Make sure docker is active
  service:
    name: docker
    state: started
    enabled: yes

#Копирую файлы из текущей директории в папку /APP
- name: Copy all files 
  become: true 
  copy:
    src: ./
    dest: "{{item}}"
    owner: ubuntu
    group: ubuntu        
    mode: 0644
  with_items:
      - "{{ App }}"

#Добавляю пользователя ubuntu в группу докер
- name: Adding a user to the Docker group
  user:
    name: ubuntu
    groups: docker
    append: yes
    
#Запускаю докер композ файл
- name: Runnig docker compose 
  command: docker-compose -f {{item}}/docker-compose.yml up -d
  register: dockercompose
  with_items:
    - "{{ App }}"
    
#Вывожу stdout запущенных докер контейнеров
- name: print to stdout
  command: docker ps
  register: docker

- debug: msg="{{ docker.stdout }}"
```

> Проверил, что всё работает теперь можно накатить всё это в пайплайн. Теперь можно объединить предыдущую [домашку](https://gitlab.com/AyuBBlack/inno/-/blob/main/CICD/HW_8.MD)

---

### В пайплайне я создаю **3 этапа**:

```
stages:      
  - Create # Для создания инфраструктуры
  - Deploy # Для деплоя всех сервисов на созданную машину
  - Destroy # Для удаления созданной машины
```
> На этапе создания инфраструктуры я экспортирую как раньше свой токен в переменную для создания инфраструктуры. После создания **вм** *(виртуальной машины)* я экспортирую внешний IP адрес вм в переменную **ip_vm**, после чего записываю эту переменную в **hosts файл**

```
Infrastructure:
  stage: Create
  script:
    - terraform init
    - export TF_VAR_IAM_TOKEN=$TOKEN
    - terraform plan -out=tfplan -input=false
    - terraform apply -input=false tfplan 
    - export ip_vm=$(terraform output external_ip_address_vm-2 | tr -d \")
    - echo "[yan]" >> hosts
    - echo $ip_vm >> hosts
    - cat hosts
```


---

На этапе деплоя пингую машину, дальше запускаю свой ансибл плэйбук **run.yml**

```
Deploy:
  stage: Deploy
  script:
    - export ip_vm=$(terraform output external_ip_address_vm-2 | tr -d \") 
    - sleep 30
    - ping -c 5 $ip_vm
    - ansible-playbook run.yml
  tags:
    - "dev"
  allow_failure: true
```
Playbook **run.yml** запускает роль, которую я описал в начале.

```
---
- name: Deploying Docker-Compose
  hosts: yan
  become: yes

  roles:
    - deploy-app
```

---
### Ну и этап удаления.

На этапе удаления у меня как из предыдущей домашки удаляется вм основываясь файл из артефакта.

```
Destroy:
  stage: Destroy
  script:
    - export TF_VAR_IAM_TOKEN=$TOKEN
    - terraform init
    - terraform destroy -auto-approve   
  tags:
    - "dev"
  when: manual
```


---

**[Ссылка на пайплайн](https://gitlab.com/AyuBBlack/inno/-/jobs/2506766352)**


---

Ну и парочка скриншотов, чтобы не быть голословным. 

![](https://i.imgur.com/iNsbwE1.png)
![](https://i.imgur.com/O9bqSUs.png)
![](https://i.imgur.com/g3HxTFE.png)

| Post запрос на API  | GET запрос на API |
| -------- | -------- |
| ![](https://i.imgur.com/DwfUllc.png)     | ![](https://i.imgur.com/gpmIi9i.png)    |