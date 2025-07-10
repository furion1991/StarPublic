'use client'

import Image from 'next/image'

import { Button, PageActions, Timer } from '@/shared/ui'
import { CurrentPointsNumber, PointsLeaderboard, QuestsList } from '@/entities/quests'

import { useAuth, useAuthModal } from '@/shared/hooks'

import classes from './Quests.module.scss'

export const QuestsPage = () => {
  const { isAuth } = useAuth()
  const { openAuthModal } = useAuthModal()

  return (
    <div className={classes.questsPage}>
      <div className={classes.pageActions}>
        <PageActions />
      </div>

      <div className={classes.wrapper}>
        <div className={classes.content}>
          <div className={classes.top}>
            <div className={classes.currentPointsNumber}>
              <CurrentPointsNumber />
            </div>

            <div className={classes.heading}>
              <h1>Ежесезонные квесты</h1>
              <h2>Выполняй задания, зарабатывай баллы и продвигайся в топе!</h2>
              <p>Чем выше место в топе — тем больше алмазов ты получишь!</p>
            </div>

            <div className={classes.giveawayEndTime}>
              <p>До конца раздачи</p>
              <Timer expiryTime='20 dec 2025 18:00:00' />
            </div>
          </div>

          {!isAuth ? (
            <Button className={classes.authBtn} onClick={openAuthModal}>
              Войти
            </Button>
          ) : null}

          {isAuth ? (
            <>
              <div className={classes.quests}>
                <QuestsList />
              </div>

              <div className={classes.leaderboard}>
                <PointsLeaderboard />
              </div>
            </>
          ) : null}
        </div>

        <Image
          className={classes.mainBgImg}
          src='/img/quests-main-bg.png'
          width={1756}
          height={889}
          quality={100}
          alt='Главный фон'
        />
      </div>
    </div>
  )
}
