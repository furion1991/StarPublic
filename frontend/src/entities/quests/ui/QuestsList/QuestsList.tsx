import { Quest } from '../Quest/Quest'

import classes from './QuestsList.module.scss'

export const QuestsList = () => {
  const quests = [
    {
      coinsReceive: 10,
      taskText: 'Пополни счёт от 30 Р',
      state: 'default' as const
    },
    {
      coinsReceive: 20,
      taskText: 'Сделай апгрейд от 80 Р',
      state: 'queued' as const
    },
    {
      coinsReceive: 35,
      taskText: 'Сделай контракт от 50 Р',
      state: 'default' as const
    },
    {
      coinsReceive: 50,
      taskText: 'Прокрути барабан 1 раз',
      state: 'default' as const
    },
    {
      coinsReceive: 100,
      taskText: 'Выбей скин от 50 Р до 150 Р',
      state: 'default' as const
    }
  ]

  return (
    <ul className={classes.questsList}>
      {quests.map(({ coinsReceive, taskText, state }) => {
        return (
          <li key={taskText}>
            <Quest key={taskText} coinsReceive={coinsReceive} taskText={taskText} state={state} />
          </li>
        )
      })}
    </ul>
  )
}
