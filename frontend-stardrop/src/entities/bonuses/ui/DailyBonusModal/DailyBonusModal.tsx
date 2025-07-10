'use client'

import Image from 'next/image'
import cn from 'classnames'

import { Modal, PriceWithCurrency } from '@/shared/ui'

import { useUser } from '@/shared/hooks'

import classes from './DailyBonusModal.module.scss'

type DailyBonusModalProps = {
  open: boolean
  onClose: () => void
  ClaimButtonSlot: React.ReactNode
}

export const DailyBonusModal = ({ open, onClose, ClaimButtonSlot }: DailyBonusModalProps) => {
  const { user } = useUser()

  if (!user) return null

  const { isUsedToday, streak, amount } = user.dailyBonus

  return (
    <Modal open={open} onClose={onClose} className={classes.dailyBonusModal}>
      <Modal.Header>
        <h4>Ежедневный бонус</h4>
        <p>Учавствуй в розыгрыше и выигрывайте призы</p>
      </Modal.Header>

      <Modal.Content>
        <div className={classes.content}>
          {!isUsedToday ? (
            <div className={classes.bonusClaim}>
              <p>Ваш ежедневный бонус</p>

              <PriceWithCurrency className={classes.bonusValue}>{amount}</PriceWithCurrency>

              {ClaimButtonSlot}

              <p>Увеличьте уровень для увеличения бонуса</p>
            </div>
          ) : (
            <div className={classes.bonusClaimSuccess}>
              <PriceWithCurrency className={classes.bonusClaimedValue}>
                + {amount}
              </PriceWithCurrency>

              <p>Бонус успешно получен</p>

              <div className={classes.bgLight} />
            </div>
          )}

          <div className={classes.bonusProgression}>
            <div className={classes.megaBonus}>
              <p>X5</p>
              <p>Мегабонус</p>
            </div>

            <div className={classes.bonusProgressionDesc}>
              <p>
                Получайте ежедневно бонусы в течении 10 дней и на 11 получи бонус в 5-ти кратном
                размере
              </p>

              <p>Когда шкала дойдёт до конца, вы получите бонус х3</p>
            </div>

            <ul className={classes.bonusProgressionList}>
              {Array.from({ length: 10 }).map((_, idx) => {
                const isActive = idx + 1 <= streak

                return (
                  <li
                    key={idx}
                    className={cn({
                      [classes.filled]: isActive
                    })}
                  >
                    <span>{idx + 1}</span>
                  </li>
                )
              })}
            </ul>
          </div>

          <div className={classes.bgArrows}>
            <Image src='/img/arrow-double.svg' width={195} height={211.5} alt='Стрелка' />
            <Image src='/img/arrow-double.svg' width={195} height={211.5} alt='Стрелка' />
          </div>
        </div>
      </Modal.Content>
    </Modal>
  )
}
