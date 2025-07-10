'use client'

import Image from 'next/image'
import { useState } from 'react'

import { Button } from '@/shared/ui'

import classes from './Quest.module.scss'

type State = 'default' | 'queued' | 'completed'

type QuestProps = {
  coinsReceive: number
  taskText: string
  state: State
}

export const Quest = ({ coinsReceive, taskText, state }: QuestProps) => {
  const [isCompleted, setCompleted] = useState(false)

  if (state === 'completed' || isCompleted)
    return (
      <div className={classes.completed}>
        <Image src='/icons/checkmark-white.svg' width={69} height={69} alt='Галочка' />

        <p>Выполнено</p>
      </div>
    )

  return (
    <div className={classes.quest}>
      <div className={classes.coins}>
        <span>{coinsReceive}</span>

        <Image src='/icons/coin.svg' width={27} height={29} alt='Монета' />
      </div>

      <p>{taskText}</p>

      {state === 'default' ? (
        <Button
          onClick={() => {
            setCompleted(true)
          }}
        >
          Начать ›
        </Button>
      ) : null}
      {state === 'queued' ? (
        <Button>
          <Image src='/icons/clock-white.svg' width={28} height={28} alt='Часы' />
        </Button>
      ) : null}
    </div>
  )
}
