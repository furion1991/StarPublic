import Image from 'next/image'

import { Modal } from '../Modal/Modal'
import { Button } from '../Button/Button'

import classes from './ConfirmModal.module.scss'

type ConfirmModalProps = {
  open: boolean
  text: string
  loading?: boolean
  onConfirm: () => void
  onClose: () => void
}

export const ConfirmModal = ({ open, text, loading, onConfirm, onClose }: ConfirmModalProps) => {
  return (
    <Modal open={open} onClose={onClose}>
      <div className={classes.confirmModal}>
        <div className={classes.actionIcon}>
          <Image src='/img/arrow-down-circle.png' width={75} height={79} alt='Стрелка вниз' />
        </div>

        <p className={classes.actionText}>{text}</p>

        <div className={classes.actions}>
          <Button loading={loading} boxShadow onClick={onConfirm}>
            Да
          </Button>

          <Button onClick={onClose}>Нет</Button>
        </div>
      </div>
    </Modal>
  )
}
