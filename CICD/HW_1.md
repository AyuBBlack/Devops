#### **Спроектировать pipeline, который должен состоять как минимум из трех этапов: Тест -> сборка -> деплой**

Этап подготовки:

1. Для начала нужно настроить раннер


**Первой задачей было:**

> Сделать запрет на мерж, если пайплайн красный

Делается это в найстроках в резделе merge requests как подсказывает [документация](https://docs.gitlab.com/ee/user/project/merge_requests/merge_when_pipeline_succeeds.html)

Поставил галочку на 

- [ ] Pipelines must succeed

И сделал маленький тест на проверку наличия директории.

```
test:
  stage: test
  variables:
    DIR: 'SqlBundle/'
  script:
     - if [ -d $DIR ]; then echo "ok"; else false; fi;
  tags:
    - "dev"
```

---

> Задача этапа сборки - собрать исполняемые файлы проекта (в случае .net - скорее всего необходимо скомпиллировать в бинарь) и выложить результат как job artifact

```
building:   
  stage: build   
  script:
    - dotnet build SqlBundle -c Release -o ./artifacts/SqlBundle
  artifacts:
    paths:
      - ./artifacts/SqlBundle  
  tags:
    - "dev"
  allow_failure: false
```

По итогу выполнения джобы готовую сборку можно скачать 

![](https://i.imgur.com/6eFCYLN.jpg)



---

> Задача этапа деплоя - необходимо вытягивать артефакт с джобы предыдущего этапа (из джобы сборки).
> Сделать это можно через api gitlab-а.
> Затем распаковывать их на выбранном раннере в директорию /project
> Джобы этого этапа должны появляться только на master-ветке вашего проекта и должны иметь ручное управление (manual).

На этом этапе я использовал predefined veriables и jobs api, чтобы скачать образ.

[Документация](https://docs.gitlab.com/ee/api/job_artifacts.html) предлагало следущий api get запрос:

```GET /projects/:id/jobs/artifacts/:ref_name/download?job=name```

И тут как раз пригодились predefined veriables, такие как __CI_PROJECT_ID__ и __CI_COMMIT_REF_NAME__.

```
Deploy:
  stage: deploy
  script:
    - curl --location --output Release.zip https://gitlab.com/api/v4/projects/$CI_PROJECT_ID/jobs/artifacts/$CI_COMMIT_REF_NAME/download?job=Release
    - mkdir ~/project
    - unzip Release -d ~/project
  when: manual
  # only: 
  #   - main
  tags:
    - "dev"
```

Также я тут использовал when: manual, чтобы запускать задачу вручную и закомиттил свое  __only: - main__ чтобы проверить работает ли задача, так пайплайн я запускал из ветки feature