'use client'

import Image from 'next/image'

import { Modal } from '@/shared/ui'

import { useGiveawayData } from '../../model/useGiveawayData'

import classes from './GiveawayBonusModal.module.scss'

type GiveawayBonusModalProps = {
  open: boolean
  onClose: () => void
  GiveawaySubscribeButtonSlot: React.ReactNode
}

export const GiveawayBonusModal = ({
  open,
  onClose,
  GiveawaySubscribeButtonSlot
}: GiveawayBonusModalProps) => {
  const { giveawayData } = useGiveawayData()

  if (!giveawayData) return

  const giveawayExpireDate = new Date(
    new Date().getTime() + giveawayData.secondsRemainingTillDraw * 1000
  )
  const remainingTime = giveawayExpireDate.getTime() - new Date().getTime()
  const remainingTimeHours = String(
    Math.floor((remainingTime % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60))
  )
  const remainingTimeMinutes = String(Math.floor((remainingTime % (1000 * 60 * 60)) / (1000 * 60)))

  const lastWinner = (
    <div className={classes.lastWinner}>
      <div className={classes.avatar}>
        <Image
          src={giveawayData.lastWinnerImageUrl || '/img/giveaway-winner-placeholder.png'}
          width={85}
          height={85}
          alt='Последний победитель'
        />
        <p className={classes.prizeValue}>{giveawayData.lastWonPrizeAmount}</p>
      </div>

      <p>Последний</p>
      <p>Победитель</p>
    </div>
  )

  return (
    <Modal open={open} onClose={onClose} className={classes.giveawayBonusModal}>
      <Modal.Header>
        <h4>Розыгрыш</h4>
        <p>Учавствуй в розыгрыше и выигрывайте призы</p>
      </Modal.Header>

      <Modal.Content>
        <div className={classes.content}>
          <div className={classes.top}>
            {lastWinner}

            <div className={classes.center}>
              <h3>
                Выиграй <span>{giveawayData.currentPrizeAmount} бонусов</span>
              </h3>

              <div className={classes.participants}>
                <p>
                  Учавствует{' '}
                  <span>{new Intl.NumberFormat('de-DE').format(giveawayData.subscribedUsers)}</span>{' '}
                  человек
                </p>
              </div>

              <div className={classes.timer}>
                <div className={classes.timerCell}>
                  {remainingTimeHours.split('').length > 1 ? remainingTimeHours.split('')[0] : 0}
                </div>

                <div className={classes.timerCell}>
                  {remainingTimeHours.split('').length > 1
                    ? remainingTimeHours.split('')[1]
                    : remainingTimeHours.split('')[0]}
                </div>

                <span>:</span>

                <div className={classes.timerCell}>
                  {remainingTimeMinutes.split('').length > 1
                    ? remainingTimeMinutes.split('')[0]
                    : 0}
                </div>

                <div className={classes.timerCell}>
                  {remainingTimeMinutes.split('').length > 1
                    ? remainingTimeMinutes.split('')[1]
                    : remainingTimeMinutes.split('')[0]}
                </div>
              </div>
            </div>

            {lastWinner}
          </div>

          {GiveawaySubscribeButtonSlot}
        </div>
      </Modal.Content>
    </Modal>
  )
}
