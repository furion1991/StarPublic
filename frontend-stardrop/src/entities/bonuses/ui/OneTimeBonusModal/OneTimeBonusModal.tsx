'use client'

import cn from 'classnames'

import { Modal, PriceWithCurrency } from '@/shared/ui'

import { useUser } from '@/shared/hooks'

import classes from './OneTimeBonusModal.module.scss'

type OneTimeBonusModalProps = {
  open: boolean
  onClose: () => void
  SocialSlots: {
    vk: React.ReactNode
    tg: React.ReactNode
  }
}

export const OneTimeBonusModal = ({ open, onClose, SocialSlots }: OneTimeBonusModalProps) => {
  const { user } = useUser()

  const tasks = [
    {
      value: 'vk',
      text: 'Подписаться на нашу страницу VK'
    },
    {
      value: 'telegram',
      text: 'Подписаться на нашу группу в TG'
    }
  ]

  return (
    <Modal open={open} onClose={onClose} className={classes.oneTimeBonusModal}>
      <Modal.Header>
        <h4>Разовый бонус</h4>
        <p>Выполняйте задания и получайте дополнительные бонусы</p>
      </Modal.Header>

      <Modal.Content>
        <div className={classes.content}>
          <ul className={classes.tasksList}>
            {tasks.map(({ value, text }) => {
              const isSubscribed =
                (value === 'telegram' && user?.isSubscribedToTg) ||
                (value === 'vk' && user?.isSubscribedToVk)

              return (
                <li key={value}>
                  <div
                    className={cn(classes.checkbox, {
                      [classes.checked]: isSubscribed
                    })}
                  >
                    <svg
                      xmlns='http://www.w3.org/2000/svg'
                      width='19'
                      height='19'
                      viewBox='0 0 19 19'
                      fill='none'
                    >
                      <path d='M7.51925 17.7395L0 10.8128L2.61666 7.97169L7.12381 12.1226L16.0105 1.26172L19 3.70797L7.51925 17.7395Z' />
                    </svg>
                  </div>

                  <p>{text}</p>

                  <PriceWithCurrency className={classes.reward} orange>
                    20
                  </PriceWithCurrency>

                  {value === 'telegram' ? SocialSlots.tg : null}
                  {value === 'vk' ? SocialSlots.vk : null}
                </li>
              )
            })}
          </ul>
        </div>
      </Modal.Content>
    </Modal>
  )
}
